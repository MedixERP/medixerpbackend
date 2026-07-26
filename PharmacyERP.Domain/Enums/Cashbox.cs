namespace PharmacyERP.Domain.Enums;

public enum CashboxTransactionType
{
    In = 1,
    Out = 2
}

public enum CashboxSource
{
    Sale = 1,
    Purchase = 2,
    Expense = 3,
    SalesReturn = 4,
    SupplierPayment = 5,
    CustomerPayment = 6
}