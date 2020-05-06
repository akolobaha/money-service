using System.ComponentModel.DataAnnotations;

namespace MoneyService.Model
{
    public class AccountModel
    {
        [Key]
        public long AccountNumber { get; set; }
        public int AccountOwnersId { get; set; }
        public int AccountBalance { get; set; }
    }
}
