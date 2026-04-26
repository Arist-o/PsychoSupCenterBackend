// API/Controllers/DoctorsController.cs
using Application.DoctorAvailabilities.DTOs;       // UpdateDoctorAvailabilityDto, AddDoctorUnavailabilityDto
using Application.DoctorCertificates.DTOs;         // UpdateDoctorCertificateDto
using Application.DoctorSpecializations.DTOs;      // UpdateSpecializationDto
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsychoSupCenterBackend.Application.DoctorAvailabilities.Commands;
using PsychoSupCenterBackend.Application.DoctorAvailabilities.DTOs;
using PsychoSupCenterBackend.Application.DoctorAvailabilities.Queries;
using PsychoSupCenterBackend.Application.DoctorCertificates.Commands;
using PsychoSupCenterBackend.Application.DoctorCertificates.DTOs;
using PsychoSupCenterBackend.Application.DoctorCertificates.Queries;
using PsychoSupCenterBackend.Application.DoctorServices.Commands;
using PsychoSupCenterBackend.Application.DoctorServices.DTOs;
using PsychoSupCenterBackend.Application.DoctorServices.Queries;
using PsychoSupCenterBackend.Application.DoctorSpecializations.Commands;
using PsychoSupCenterBackend.Application.DoctorSpecializations.DTOs;
using PsychoSupCenterBackend.Application.DoctorSpecializations.Queries;
using PsychoSupCenterBackend.Application.Doctors.Commands;
using PsychoSupCenterBackend.Application.Doctors.DTOs;
using PsychoSupCenterBackend.Application.Doctors.Queries;
using PsychoSupCenterBackend.Domain.Enums;
using Application.Doctors.DTOs;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DoctorsController(ISender sender) : ControllerBase
{
    // ════════════════════════════════════════════════════════════
    // DOCTOR PROFILE
    // ════════════════════════════════════════════════════════════

    /// <summary>GET /api/doctors</summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<DoctorProfileResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] DoctorStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetAllDoctors.Query(status, page, pageSize), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    /// <summary>GET /api/doctors/{doctorProfileId}</summary>
    [HttpGet("{doctorProfileId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(DoctorProfileResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid doctorProfileId,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetDoctorById.Query(doctorProfileId), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    /// <summary>GET /api/doctors/by-specialization?specializationName=...</summary>
    [HttpGet("by-specialization")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<DoctorProfileResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBySpecialization(
        [FromQuery] string specializationName,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(
            new GetDoctorsBySpecialization.Query(specializationName, page, pageSize),
            cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    /// <summary>POST /api/doctors?userId=... — Створити профіль лікаря</summary>
    [HttpPost]
    [ProducesResponseType(typeof(DoctorProfileResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromQuery] Guid userId,
        [FromBody] CreateDoctorProfileDto dto,
        CancellationToken cancellationToken)
    {
        // Command(Guid UserId, CreateDoctorProfileDto Dto)
        var result = await sender.Send(new CreateDoctorProfile.Command(userId, dto), cancellationToken);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { doctorProfileId = result.Value!.Id }, result.Value)
            : BadRequest(new { error = result.Error });
    }

    /// <summary>PUT /api/doctors/{doctorProfileId}</summary>
    [HttpPut("{doctorProfileId:guid}")]
    [ProducesResponseType(typeof(DoctorProfileResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid doctorProfileId,
        [FromBody] UpdateDoctorProfileDto dto,
        CancellationToken cancellationToken)
    {
        // Command(Guid DoctorProfileId, UpdateDoctorProfileDto Dto)
        var result = await sender.Send(new UpdateDoctorProfile.Command(doctorProfileId, dto), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    /// <summary>PATCH /api/doctors/{doctorProfileId}/status</summary>
    [HttpPatch("{doctorProfileId:guid}/status")]
    [ProducesResponseType(typeof(DoctorProfileResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangeStatus(
        [FromRoute] Guid doctorProfileId,
        [FromBody] ChangeDoctorStatusDto dto,
        CancellationToken cancellationToken)
    {
        // Command(Guid DoctorProfileId, ChangeDoctorStatusDto Dto)
        var result = await sender.Send(new ChangeDoctorStatus.Command(doctorProfileId, dto), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    /// <summary>DELETE /api/doctors/{doctorProfileId}</summary>
    [HttpDelete("{doctorProfileId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid doctorProfileId,
        CancellationToken cancellationToken)
    {
        // Command(Guid DoctorProfileId)
        var result = await sender.Send(new DeleteDoctorProfile.Command(doctorProfileId), cancellationToken);
        return result.IsSuccess ? Ok(new { message = "Профіль лікаря видалено." }) : BadRequest(new { error = result.Error });
    }

    // ════════════════════════════════════════════════════════════
    // DOCTOR SERVICES
    // ════════════════════════════════════════════════════════════

    /// <summary>GET /api/doctors/{doctorProfileId}/services</summary>
    [HttpGet("{doctorProfileId:guid}/services")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<DoctorServiceResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetServices(
        [FromRoute] Guid doctorProfileId,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetServicesByDoctorId.Query(doctorProfileId), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    /// <summary>GET /api/doctors/services/active</summary>
    [HttpGet("services/active")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<DoctorServiceResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllActiveServices(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetAllActiveServices.Query(page, pageSize), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    /// <summary>GET /api/doctors/services/{serviceId}</summary>
    [HttpGet("services/{serviceId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(DoctorServiceResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetServiceById(
        [FromRoute] Guid serviceId,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetDoctorServiceById.Query(serviceId), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    /// <summary>POST /api/doctors/{doctorProfileId}/services</summary>
    [HttpPost("{doctorProfileId:guid}/services")]
    [ProducesResponseType(typeof(DoctorServiceResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateService(
        [FromRoute] Guid doctorProfileId,
        [FromBody] CreateDoctorServiceDto dto,      // ← Application DTO: ServiceName + Price
        CancellationToken cancellationToken)
    {
        // Command(Guid DoctorProfileId, CreateDoctorServiceDto Dto)  ← 2 аргументи
        var result = await sender.Send(
            new CreateDoctorService.Command(doctorProfileId, dto),
            cancellationToken);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetServiceById), new { serviceId = result.Value!.Id }, result.Value)
            : BadRequest(new { error = result.Error });
    }

    /// <summary>PUT /api/doctors/services/{serviceId}</summary>
    [HttpPut("services/{serviceId:guid}")]
    [ProducesResponseType(typeof(DoctorServiceResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateService(
        [FromRoute] Guid serviceId,
        [FromBody] UpdateDoctorServiceDto dto,      // ← Application DTO: ServiceName + Price
        CancellationToken cancellationToken)
    {
        // Command(Guid ServiceId, UpdateDoctorServiceDto Dto)  ← 2 аргументи
        var result = await sender.Send(
            new UpdateDoctorService.Command(serviceId, dto),
            cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    /// <summary>DELETE /api/doctors/services/{serviceId}</summary>
    [HttpDelete("services/{serviceId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteService(
        [FromRoute] Guid serviceId,
        CancellationToken cancellationToken)
    {
        // Command(Guid ServiceId)
        var result = await sender.Send(new DeleteDoctorService.Command(serviceId), cancellationToken);
        return result.IsSuccess ? Ok(new { message = "Послугу видалено." }) : BadRequest(new { error = result.Error });
    }

    // ════════════════════════════════════════════════════════════
    // DOCTOR SPECIALIZATIONS
    // ════════════════════════════════════════════════════════════

    /// <summary>GET /api/doctors/specializations</summary>
    [HttpGet("specializations")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<SpecializationResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllSpecializations(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetAllSpecializations.Query(page, pageSize), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    /// <summary>GET /api/doctors/specializations/{specializationId}</summary>
    [HttpGet("specializations/{specializationId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(SpecializationResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSpecializationById(
        [FromRoute] Guid specializationId,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetSpecializationById.Query(specializationId), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    /// <summary>GET /api/doctors/{doctorProfileId}/specializations</summary>
    [HttpGet("{doctorProfileId:guid}/specializations")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<SpecializationResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSpecializationsByDoctor(
        [FromRoute] Guid doctorProfileId,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetSpecializationsByDoctorId.Query(doctorProfileId), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    /// <summary>POST /api/doctors/{doctorProfileId}/specializations — Призначити спеціалізацію</summary>
    [HttpPost("{doctorProfileId:guid}/specializations")]
    [ProducesResponseType(typeof(SpecializationResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AssignSpecialization(
        [FromRoute] Guid doctorProfileId,
        [FromBody] AssignSpecializationDto dto,     // ← Application DTO: DoctorProfileId + Name
        CancellationToken cancellationToken)
    {
        // Command(AssignSpecializationDto Dto) — doctorProfileId з route ігнорується,
        // береться з dto.DoctorProfileId. Щоб уникнути розбіжностей — перебудовуємо dto.
        var enrichedDto = dto with { DoctorProfileId = doctorProfileId };
        var result = await sender.Send(
            new AssignSpecializationToDoctor.Command(enrichedDto),
            cancellationToken);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetSpecializationById), new { specializationId = result.Value!.Id }, result.Value)
            : BadRequest(new { error = result.Error });
    }

    /// <summary>PUT /api/doctors/specializations/{specializationId}</summary>
    [HttpPut("specializations/{specializationId:guid}")]
    [ProducesResponseType(typeof(SpecializationResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateSpecialization(
        [FromRoute] Guid specializationId,
        [FromBody] UpdateSpecializationDto dto,     // ← Application DTO з namespace Application.DoctorSpecializations.DTOs
        CancellationToken cancellationToken)
    {
        // Command(Guid SpecializationId, UpdateSpecializationDto Dto)
        var result = await sender.Send(
            new UpdateSpecialization.Command(specializationId, dto),
            cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    /// <summary>DELETE /api/doctors/specializations/{specializationId}</summary>
    [HttpDelete("specializations/{specializationId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteSpecialization(
        [FromRoute] Guid specializationId,
        CancellationToken cancellationToken)
    {
        // Command(Guid SpecializationId)
        var result = await sender.Send(new DeleteSpecialization.Command(specializationId), cancellationToken);
        return result.IsSuccess ? Ok(new { message = "Спеціалізацію видалено." }) : BadRequest(new { error = result.Error });
    }

    /// <summary>DELETE /api/doctors/{doctorProfileId}/specializations?name=...</summary>
    [HttpDelete("{doctorProfileId:guid}/specializations")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoveSpecializationFromDoctor(
        [FromRoute] Guid doctorProfileId,
        [FromQuery] string name,
        CancellationToken cancellationToken)
    {
        // Command(Guid DoctorProfileId, string Name)
        var result = await sender.Send(
            new RemoveSpecializationFromDoctor.Command(doctorProfileId, name),
            cancellationToken);
        return result.IsSuccess ? Ok(new { message = "Спеціалізацію знято." }) : BadRequest(new { error = result.Error });
    }

    // ════════════════════════════════════════════════════════════
    // DOCTOR AVAILABILITY
    // ════════════════════════════════════════════════════════════

    /// <summary>GET /api/doctors/{doctorProfileId}/schedule</summary>
    [HttpGet("{doctorProfileId:guid}/schedule")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<DoctorAvailabilityResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSchedule(
        [FromRoute] Guid doctorProfileId,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetScheduleByDoctorId.Query(doctorProfileId), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    /// <summary>GET /api/doctors/availabilities/{availabilityId}</summary>
    [HttpGet("availabilities/{availabilityId:guid}")]
    [ProducesResponseType(typeof(DoctorAvailabilityResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAvailabilityById(
        [FromRoute] Guid availabilityId,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetDoctorAvailabilityById.Query(availabilityId), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    /// <summary>POST /api/doctors/{doctorProfileId}/availabilities</summary>
    [HttpPost("{doctorProfileId:guid}/availabilities")]
    [ProducesResponseType(typeof(DoctorAvailabilityResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAvailability(
        [FromRoute] Guid doctorProfileId,
        [FromBody] CreateDoctorAvailabilityDto dto, // ← Application DTO: DoctorProfileId + Day + StartTime + EndTime
        CancellationToken cancellationToken)
    {
        // Command(CreateDoctorAvailabilityDto Dto) — 1 аргумент
        // Перебудовуємо dto з doctorProfileId з route
        var enrichedDto = dto with { DoctorProfileId = doctorProfileId };
        var result = await sender.Send(
            new CreateDoctorAvailability.Command(enrichedDto),
            cancellationToken);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetAvailabilityById), new { availabilityId = result.Value!.Id }, result.Value)
            : BadRequest(new { error = result.Error });
    }

    /// <summary>PUT /api/doctors/availabilities/{availabilityId}</summary>
    [HttpPut("availabilities/{availabilityId:guid}")]
    [ProducesResponseType(typeof(DoctorAvailabilityResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateAvailability(
        [FromRoute] Guid availabilityId,
        [FromBody] UpdateDoctorAvailabilityDto dto, // ← Application DTO: Day + StartTime + EndTime
        CancellationToken cancellationToken)
    {
        // Command(Guid AvailabilityId, UpdateDoctorAvailabilityDto Dto)  ← 2 аргументи
        var result = await sender.Send(
            new UpdateDoctorAvailability.Command(availabilityId, dto),
            cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    /// <summary>DELETE /api/doctors/availabilities/{availabilityId}</summary>
    [HttpDelete("availabilities/{availabilityId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteAvailability(
        [FromRoute] Guid availabilityId,
        CancellationToken cancellationToken)
    {
        // Command(Guid AvailabilityId)
        var result = await sender.Send(new DeleteDoctorAvailability.Command(availabilityId), cancellationToken);
        return result.IsSuccess ? Ok(new { message = "Слот видалено." }) : BadRequest(new { error = result.Error });
    }

    /// <summary>POST /api/doctors/{doctorProfileId}/unavailabilities — Відпустка/лікарняний</summary>
    [HttpPost("{doctorProfileId:guid}/unavailabilities")]
    [ProducesResponseType(typeof(DoctorUnavailabilityResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddUnavailability(
        [FromRoute] Guid doctorProfileId,
        [FromBody] AddDoctorUnavailabilityDto dto,  // ← Application DTO: DoctorProfileId + StartDate + EndDate + Reason
        CancellationToken cancellationToken)
    {
        // Command(AddDoctorUnavailabilityDto Dto) — 1 аргумент
        var enrichedDto = dto with { DoctorProfileId = doctorProfileId };
        var result = await sender.Send(
            new AddDoctorUnavailability.Command(enrichedDto),
            cancellationToken);
        return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, result.Value)
            : BadRequest(new { error = result.Error });
    }

    /// <summary>PUT /api/doctors/unavailabilities/{unavailabilityId}</summary>
    [HttpPut("unavailabilities/{unavailabilityId:guid}")]
    [ProducesResponseType(typeof(DoctorUnavailabilityResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateUnavailability(
        [FromRoute] Guid unavailabilityId,
        [FromBody] UpdateUnavailabilityBodyDto body,
        CancellationToken cancellationToken)
    {
        // Command(Guid UnavailabilityId, UpdateUnavailabilityBodyDto Dto) — 2 аргументи
        var result = await sender.Send(
            new UpdateDoctorUnavailability.Command(unavailabilityId, body),
            cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    /// <summary>DELETE /api/doctors/unavailabilities/{unavailabilityId}</summary>
    [HttpDelete("unavailabilities/{unavailabilityId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteUnavailability(
        [FromRoute] Guid unavailabilityId,
        CancellationToken cancellationToken)
    {
        // Command(Guid UnavailabilityId)
        var result = await sender.Send(new DeleteDoctorUnavailability.Command(unavailabilityId), cancellationToken);
        return result.IsSuccess ? Ok(new { message = "Запис про відсутність видалено." }) : BadRequest(new { error = result.Error });
    }

    // ════════════════════════════════════════════════════════════
    // DOCTOR CERTIFICATES
    // ════════════════════════════════════════════════════════════

    /// <summary>GET /api/doctors/{doctorProfileId}/certificates</summary>
    [HttpGet("{doctorProfileId:guid}/certificates")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<DoctorCertificateResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCertificates(
        [FromRoute] Guid doctorProfileId,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetCertificatesByDoctorId.Query(doctorProfileId), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    /// <summary>GET /api/doctors/certificates/{certificateId}</summary>
    [HttpGet("certificates/{certificateId:guid}")]
    [ProducesResponseType(typeof(DoctorCertificateResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCertificateById(
        [FromRoute] Guid certificateId,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetDoctorCertificateById.Query(certificateId), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    /// <summary>POST /api/doctors/{doctorProfileId}/certificates</summary>
    [HttpPost("{doctorProfileId:guid}/certificates")]
    [ProducesResponseType(typeof(DoctorCertificateResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddCertificate(
        [FromRoute] Guid doctorProfileId,
        [FromBody] AddDoctorCertificateDto dto,     // ← Application DTO: DoctorProfileId + CertificateUrl
        CancellationToken cancellationToken)
    {
        // Command(AddDoctorCertificateDto Dto) — 1 аргумент
        var enrichedDto = dto with { DoctorProfileId = doctorProfileId };
        var result = await sender.Send(
            new AddDoctorCertificate.Command(enrichedDto),
            cancellationToken);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetCertificateById), new { certificateId = result.Value!.Id }, result.Value)
            : BadRequest(new { error = result.Error });
    }

    /// <summary>PUT /api/doctors/certificates/{certificateId}</summary>
    [HttpPut("certificates/{certificateId:guid}")]
    [ProducesResponseType(typeof(DoctorCertificateResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateCertificate(
        [FromRoute] Guid certificateId,
        [FromBody] UpdateDoctorCertificateDto dto,  // ← Application DTO з namespace Application.DoctorCertificates.DTOs
        CancellationToken cancellationToken)
    {
        // Command(Guid CertificateId, UpdateDoctorCertificateDto Dto)  ← 2 аргументи
        var result = await sender.Send(
            new UpdateDoctorCertificate.Command(certificateId, dto),
            cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    /// <summary>DELETE /api/doctors/certificates/{certificateId}</summary>
    [HttpDelete("certificates/{certificateId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoveCertificate(
        [FromRoute] Guid certificateId,
        CancellationToken cancellationToken)
    {
        // Command(Guid CertificateId)
        var result = await sender.Send(new RemoveDoctorCertificate.Command(certificateId), cancellationToken);
        return result.IsSuccess ? Ok(new { message = "Сертифікат видалено." }) : BadRequest(new { error = result.Error });
    }
}

