using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechMoveApp.Models
{
    public enum ContractStatus { Draft, Active, Expired, OnHold }

    public class Client
    {
        [Key] public int ClientId { get; set; }
        [Required, StringLength(100)] public string Name { get; set; } = string.Empty;
        [Required, StringLength(255)] public string ContactDetails { get; set; } = string.Empty;
        [Required, StringLength(50)] public string Region { get; set; } = string.Empty;
        public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();
    }

    public class Contract
    {
        [Key] public int ContractId { get; set; }
        [Required] public int ClientId { get; set; }
        [ForeignKey("ClientId")] public virtual Client? Client { get; set; }
        [Required, DataType(DataType.Date)] public DateTime StartDate { get; set; }
        [Required, DataType(DataType.Date)] public DateTime EndDate { get; set; }
        [Required] public ContractStatus Status { get; set; }
        [Required, StringLength(50)] public string ServiceLevel { get; set; } = string.Empty;
        public string? SignedAgreementFilePath { get; set; }
        public virtual ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
    }

    public class ServiceRequest
    {
        [Key] public int ServiceRequestId { get; set; }
        [Required] public int ContractId { get; set; }
        [ForeignKey("ContractId")] public virtual Contract? Contract { get; set; }
        [Required] public string Description { get; set; } = string.Empty;
        [Required, Column(TypeName = "decimal(18,2)")] public decimal CostUSD { get; set; }
        [Required, Column(TypeName = "decimal(18,2)")] public decimal CostZAR { get; set; }
        [Required, StringLength(50)] public string Status { get; set; } = "Pending";
    }
}