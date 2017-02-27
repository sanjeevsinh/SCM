using System;
using System.Collections.Generic;
using System.Linq;
using SCM.Data;
using SCM.Models;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace SCM.Services.SCMServices
{
    public class AttachmentService : BaseService, IAttachmentService
    {

        public AttachmentService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public async Task<TenantAttachments> GetByTenantAsync(Tenant tenant)
        {
            var tenantAttachments = new TenantAttachments();
            var ifaces = await UnitOfWork.InterfaceRepository.GetAsync(q => q.Port.TenantID == tenant.TenantID, 
                includeProperties: "Port.Device.Location.SubRegion.Region,Port.Interface.Vrf,Port.Interface.InterfaceBandwidth", AsTrackable: false);

            tenantAttachments.AttachmentInterfaces = Mapper.Map<List<AttachmentInterface>>(ifaces);

            var bundleIfaces = await UnitOfWork.BundleInterfaceRepository.GetAsync(q => q.BundleInterfaceID == tenant.TenantID,
                includeProperties: "Vrf", AsTrackable: false);

            tenantAttachments.AttachmentBundleInterfaces = Mapper.Map<List<AttachmentBundleInterface>>(bundleIfaces);

            return tenantAttachments;
        }

        public async Task<AttachmentRequest> GetAttachmentInterfaceByIDAsync(int id)
        {
            var iface = await UnitOfWork.InterfaceRepository.GetAsync(q => q.ID == id, includeProperties: "Vrf", AsTrackable: false);
            return Mapper.Map<AttachmentRequest>(iface);
        }

        public async Task<AttachmentRequest> GetAttachmentBundleInterfaceByIDAsync(int id)
        {
            var bundleIface = await UnitOfWork.BundleInterfaceRepository.GetAsync(q => q.BundleInterfaceID == id, includeProperties: "Vrf", AsTrackable: false);
            return Mapper.Map<AttachmentRequest>(bundleIface);
        }

        public async Task<ServiceResult> AddAsync(AttachmentRequest attachmentRequest)
        {
            var serviceResult = new ServiceResult();
            serviceResult.IsSuccess = true;

            var devices = await UnitOfWork.DeviceRepository.GetAsync(q => q.LocationID == attachmentRequest.LocationID, 
                includeProperties:"Ports.PortBandwidth");

            if (attachmentRequest.PlaneID != null)
            {
                devices = devices.Where(q => q.PlaneID == attachmentRequest.PlaneID).ToList();
            }

            var bandwidth = await UnitOfWork.InterfaceBandwidthRepository.GetByIDAsync(attachmentRequest.BandwidthID);
            devices = devices.Where(q => q.Ports.Where(p => p.PortBandwidth.BandwidthGbps == bandwidth.BandwidthGbps).Count() > 0).ToList();

            Device device = null;

            if (devices.Count == 0)
            {
                serviceResult.Add("A device matching the requested paramaters could not be found. "
                    + "Please contact your system adminstrator and report this issue.");

                serviceResult.IsSuccess = false;

                return serviceResult;
            }
            else if (devices.Count > 1)
            {
                // Get device with the least number of tenant-assigned ports.

                device = devices.Aggregate((current, x) => 
                (x.Ports.Where(p => p.TenantID != null).Count() < current.Ports.Where(p => p.TenantID != null).Count() ? x : current));
            }
            else
            {
                device = devices.Single();
            }

            var port = device.Ports.First(q => q.TenantID == null && q.PortBandwidth.BandwidthGbps == bandwidth.BandwidthGbps);
            if (port == null)
            {
                serviceResult.Add("A port matching the requested bandwidth parameter could not be found. "
                    + "Please contact your system adminstrator and report this issue.");

                serviceResult.IsSuccess = false;

                return serviceResult;
            }

            port.TenantID = attachmentRequest.TenantID;

            var iface = Mapper.Map<Interface>(attachmentRequest);
            iface.ID = port.ID;

            Vrf vrf = null;
            if (attachmentRequest.IsLayer3)
            {
                vrf = Mapper.Map<Vrf>(attachmentRequest);
                vrf.DeviceID = device.ID;
            }

            // Need to implement Transaction Scope here when available in dotnet core

            try
            {
                UnitOfWork.PortRepository.Update(port);
                if (vrf != null)
                {
                    UnitOfWork.VrfRepository.Insert(vrf);
                    await this.UnitOfWork.SaveAsync();
                    iface.VrfID = vrf.VrfID;
                }
                UnitOfWork.InterfaceRepository.Insert(iface);
                await this.UnitOfWork.SaveAsync();
            }

            catch (Exception /** ex **/) 
            {
                // Add logging for the exception here
                serviceResult.Add("Something went wrong during the database update. The issue has been logged."
                   + "Please try again, and contact your system admin if the problem persists.");
                serviceResult.IsSuccess = false;

                return serviceResult;
            }

            return serviceResult;
        }

        public async Task<ServiceResult> DeleteAttachmentInterfaceAsync(AttachmentInterface attachment)
        {
            var result = new ServiceResult();
            result.IsSuccess = true;

            var port = await UnitOfWork.PortRepository.GetByIDAsync(attachment.ID);
            port.TenantID = null;

            try
            {
                UnitOfWork.PortRepository.Update(port);
                UnitOfWork.InterfaceRepository.DeleteAsync(attachment.ID);
                if (attachment.VrfID != null)
                {
                    UnitOfWork.VrfRepository.DeleteAsync(attachment.VrfID);
                }
            }
            catch (DbUpdateException  /* ex */)
            {
                result.Add("The delete operation failed. The error has been logged. "
                   + "Please try again and contact your system administrator if the problem persists.");

                result.IsSuccess = false;
            }

            return result;     
        }

        public async Task<ServiceResult> DeleteAttachmentBundleInterfaceAsync(AttachmentBundleInterface attachment)
        {
            throw new NotImplementedException();
        }
    }
}