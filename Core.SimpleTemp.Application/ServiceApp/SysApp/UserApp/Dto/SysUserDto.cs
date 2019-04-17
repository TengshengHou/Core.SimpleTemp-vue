using Core.SimpleTemp.Entitys;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace Core.SimpleTemp.Application.UserApp
{
    public class SysUserDto : Dto
    {
      
        public string LoginName { get; set; }
       
        public string Password { get; set; }
        public string Name { get; set; }

        public DateTime LastUpdate { get; set; }
        public int? Sex { get; set; }
        public string Remarks { get; set; }



        public Guid SysDepartmentId { get; set; }
        
        public SysDepartment SysDepartment { get; set; }

        public virtual ICollection<SysUserRoleDto> UserRoles { get; set; }
        public virtual ICollection<Guid> RolesId { get; set; }
       
    }
}
