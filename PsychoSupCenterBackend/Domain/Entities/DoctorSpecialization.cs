using PsychoSupCenterBackend.Domain.Common;

namespace PsychoSupCenterBackend.Domain.Entities;

public class DoctorSpecialization : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ICollection<DoctorProfile> DoctorProfiles { get; set; } = [];
}