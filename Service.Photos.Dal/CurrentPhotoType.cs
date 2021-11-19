using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service.Photos.Dal
{
    [Table("CURRENT_PHOTO_TYPE")]
    public class CurrentPhotoType
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [Column("START_DATE")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [Column("TYPE_ID")]
        public int TypeId { get; set; }

        [Column("PARENT_ID")]
        public int? ParentId { get; set; }

        [Required]
        [StringLength(255)]
        [Column("CODE", TypeName = "NVARCHAR2")]
        public string Code { get; set; }

        [Required]
        [StringLength(255)]
        [Column("NAME", TypeName = "NVARCHAR2")]
        public string Name { get; set; }

        [Column("ORDER")]
        public int Order { get; set; }

        [Column("DOC_TYPE_ID")]
        public DocumentType? DocumentType { get; set; }
    }
}
