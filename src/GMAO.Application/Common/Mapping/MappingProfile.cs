using AutoMapper;
using GMAO.Application.DTOs;
using GMAO.Domain.Entities.Securite;

namespace GMAO.Application.Common.Mapping;

/// <summary>Profil AutoMapper : conversions entités ↔ DTO.</summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Utilisateur, UtilisateurDto>()
            .ForMember(d => d.NomComplet, o => o.MapFrom(s => s.NomComplet))
            .ForMember(d => d.RoleLibelle, o => o.MapFrom(s => RoleLibelle.Pour(s.Role)));
    }
}
