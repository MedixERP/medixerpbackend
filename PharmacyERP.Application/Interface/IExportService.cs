public interface IExportService
{
    byte[] ExportToPdf<T>(List<T> data, string title);
    byte[] ExportToExcel<T>(List<T> data, string sheetName);
}