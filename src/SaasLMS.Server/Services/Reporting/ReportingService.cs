    private void AddMetricRow(IContainer table, string metric, object value, string? change = null)
    {
        table.Cell().Text(metric);
        table.Cell().AlignRight().Text(value.ToString());
        table.Cell().AlignRight().Text(change ?? "-");
    }

    private string FormatTimeSpan(TimeSpan time)
    {
        if (time.TotalDays >= 1)
            return $"{time.Days}d {time.Hours}h";
        if (time.TotalHours >= 1)
            return $"{time.Hours}h {time.Minutes}m";
        return $"{time.Minutes}m";
    }

    private async Task<byte[]> GenerateExcelReport(CourseAnalyticsDTO analytics)
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Course Analytics");

        // Add headers
        worksheet.Cells["A1"].Value = "Metric";
        worksheet.Cells["B1"].Value = "Value";

        // Add data
        int row = 2;
        worksheet.Cells[$"A{row}"].Value = "Total Enrollments";
        worksheet.Cells[$"B{row++}"].Value = analytics.TotalEnrollments;

        worksheet.Cells[$"A{row}"].Value = "Completion Rate";
        worksheet.Cells[$"B{row++}"].Value = analytics.CompletionRate;

        worksheet.Cells[$"A{row}"].Value = "Average Rating";
        worksheet.Cells[$"B{row++}"].Value = analytics.AverageRating;

        worksheet.Cells[$"A{row}"].Value = "Total Revenue";
        worksheet.Cells[$"B{row++}"].Value = analytics.TotalRevenue;

        // Add enrollment trend
        row += 2;
        worksheet.Cells[$"A{row}"].Value = "Enrollment Trend";
        row++;
        worksheet.Cells[$"A{row}"].Value = "Date";
        worksheet.Cells[$"B{row}"].Value = "Enrollments";

        foreach (var trend in analytics.EnrollmentTrend)
        {
            row++;
            worksheet.Cells[$"A{row}"].Value = trend.Key;
            worksheet.Cells[$"B{row}"].Value = trend.Value;
        }

        // Auto-fit columns
        worksheet.Cells.AutoFitColumns();

        return await package.GetAsByteArrayAsync();
    }
}