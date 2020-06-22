using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MTProvider.Models
{
    

    public class MTAccounts
    {
        [Key]
        public string UserName { get; set; }

        public string Password { get; set; }

        public double? Volume { get; set; }
    }

    public class MTSymbols
    {
        [Key]
        public string Name { get; set; }

        public double? AskPrice { get; set; }

        public double? BidPrice { get; set; }
    }

    [Table("SymbolsEnable")]
    public class SymbolsEnable
    {

        [Key, Column(Order = 1)]
        public string SymbolName { get; set; }

        [Key, Column(Order = 2)]
        public string UserName { get; set; }

        public bool? Status {
            get
            {
                if (StatusTri == TriState.Indeterminate) return null;
                return StatusTri == TriState.Checked;
            }
            set
            {
                switch (value)
                {
                    case null: StatusTri = TriState.Indeterminate; break;
                    case true: StatusTri = TriState.Checked; break;
                    case false: StatusTri = TriState.Unchecked; break;
                }
            }
        }

        [NotMapped]
        public TriState StatusTri { get; set; }
    }

    public class Positions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        public int RowNO { get; set; }

        public int TicketNO { get; set; }

        public string SymbolName { get; set; }

        public double Volume { get; set; }

        public string UserName { get; set; }

        public bool PositionStatus { get; set; }

        public bool IsClose { get; set; }

        public bool PositionState { get; set; }

        public double? Price { get; set; }

        public double? ClosePrice { get; set; }

        public DateTime ExpireDate { get; set; }

        [NotMapped]
        public double? Profit { get; set; }

        [NotMapped]
        public double? MarketPrice { get; set; }
    }

    public class PositionsDTO
    {
        [Key]
        public long ID { get; set; }

        public int RowNO { get; set; }

        public int TicketNO { get; set; }

        public string SymbolName { get; set; }

        public double Volume { get; set; }

        public string UserName { get; set; }

        public bool PositionStatus { get; set; }

        public bool IsClose { get; set; }

        public bool PositionState { get; set; }

        public double? Price { get; set; }

        public double? ClosePrice { get; set; }

        public DateTime ExpireDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = true)]
        public double? Profit { get; set; }

        public double? MarketPrice { get; set; }
    }
}