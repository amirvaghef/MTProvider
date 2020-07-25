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

        public string BrokerName { get; set; }

        public short Leverage { get; set; }
    }

    public class MTSymbols
    {
        [Key]
        public string Name { get; set; }

        public double? AskPrice { get; set; }

        public double? BidPrice { get; set; }

        public double? PerUSDAsk { get; set; }

        public double? PerUSDBid { get; set; }

        public string ChngFromUSDSymbol { get; set; }

        public string ChngToUSDSymbol { get; set; }
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

        public short Leverage { get; set; }

        [NotMapped]
        public double? Profit { get; set; }

        [NotMapped]
        public double? MarketPrice { get; set; }

        public double? PriceInUSD { get; set; }

        public string PriceInUSDSymbol { get; set; }
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

        [Display(Name ="Deal Price")]
        public double? Price { get; set; }

        public double? ClosePrice { get; set; }

        public DateTime ExpireDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = true)]
        public double? Profit { get; set; }

        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = true)]
        public double? ProfitPercent { get; set; }

        public double? MarketPrice { get; set; }

        public short Leverage { get; set; }
    }

    public class MTHistoryMSDTO
    {
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        //public MTHistoryMS()
        //{
        //    this.MTHistoryDTs = new HashSet<MTHistoryDT>();
        //}

        [Key]
        public short ID { get; set; }
        public string Symbol { get; set; }
        public Nullable<int> Period { get; set; }
        public Nullable<System.DateTime> StartTime { get; set; }
        public Nullable<bool> AutoBuySell { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<MTHistoryDT> MTHistoryDTs { get; set; }
        //public virtual MTPeriod MTPeriod { get; set; }
        //public virtual MTSymbols MTSymbol { get; set; }
    }

    [Table("MTPeriod")]
    public class MTPeriod
    {
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        //public MTPeriod()
        //{
        //    this.MTHistoryMS = new HashSet<MTHistoryMS>();
        //}

        [Key]
        public int PeriodID { get; set; }
        public string PeriodName { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<MTHistoryMS> MTHistoryMS { get; set; }
    }

    [Table("MTHistoryMS")]
    public class MTHistoryMS
    {
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        //public MTHistoryMS()
        //{
        //    this.MTHistoryDTs = new HashSet<MTHistoryDT>();
        //}

        [Key]
        public short ID { get; set; }
        public string Symbol { get; set; }
        public Nullable<int> Period { get; set; }
        public Nullable<System.DateTime> StartTime { get; set; }
        public Nullable<bool> AutoBuySell { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<MTHistoryDT> MTHistoryDTs { get; set; }
        //public virtual MTPeriod MTPeriod { get; set; }
        //public virtual MTSymbols MTSymbol { get; set; }
    }

    [Table("MTHistoryDT")]
    public class MTHistoryDT
    {
        [Key]
        [Timestamp]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte[] ID { get; set; }
        public Nullable<short> MSID { get; set; }
        public Nullable<System.DateTime> Time { get; set; }
        public Nullable<double> Open { get; set; }
        public Nullable<double> Close { get; set; }
        public Nullable<double> Low { get; set; }
        public Nullable<double> High { get; set; }
        public Nullable<double> Volume { get; set; }

        //public virtual MTHistoryMS MTHistoryMS { get; set; }
    }
}