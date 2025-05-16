using AutoFixture.Xunit2;
using FluentAssertions;
using FluentResults;
using Moq;
using ReportService.Projections;
using ReportService.Repositories;
using ReportService.Services;
using Shouldly;
using Xunit;

namespace ReportService.Tests;

public class ReportServiceTests
{
    [Theory]
    [AutoMoqData]
    public async Task GenerateReportAsync_ShouldReturnSuccessResult_WhenAllDependenciesSucceed(
        [Frozen] Mock<IEmployeeCodeProvider> employeeCodeProviderMock,
        [Frozen] Mock<IEmployeeSalaryProvider> employeeSalaryProviderMock,
        [Frozen] Mock<IEmployeeRepository> employeeRepositoryMock,
        [Frozen] Mock<IMonthNameResolver> monthNameResolverMock,
        Services.ReportService sut)
    {
        // Arrange
        const int year = 2023;
        const int month = 5;
        const string monthName = "May 2023";

        var departmentId1 = 1L;
        var departmentId2 = 2L;

        var employee1 = new EmployeeReportProjection
        {
            Name = "John Doe",
            Inn = "123456789",
            DepartmentId = departmentId1,
            DepartmentName = "IT"
        };

        var employee2 = new EmployeeReportProjection
        {
            Name = "Jane Smith",
            Inn = "987654321",
            DepartmentId = departmentId1,
            DepartmentName = "IT"
        };

        var employee3 = new EmployeeReportProjection
        {
            Name = "Bob Johnson",
            Inn = "456789123",
            DepartmentId = departmentId2,
            DepartmentName = "Finance"
        };

        var employees = new[] {employee1, employee2, employee3};
        var employeeLookup = employees.ToLookup(e => e.DepartmentId);

        monthNameResolverMock
            .Setup(m => m.GetName(year, month))
            .Returns(monthName);

        employeeRepositoryMock
            .Setup(m => m.GetEmployeesForAllActiveDepartmentsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(employeeLookup);

        employeeCodeProviderMock
            .Setup(m => m.GetCode(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok("BUH123"));

        employeeSalaryProviderMock
            .Setup(m => m.GetSalary(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(50000));

        // Act
        var result = await sut.GenerateReportAsync(year, month, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Should()
            .NotBeNullOrEmpty();

        // Verify that the month name resolver was called with the correct parameters
        monthNameResolverMock.Verify(m => m.GetName(year, month), Times.Once);

        // Verify that the employee repository was called
        employeeRepositoryMock.Verify(m => m.GetEmployeesForAllActiveDepartmentsAsync(It.IsAny<CancellationToken>()), Times.Once);

        // Verify that the employee code provider was called for each employee
        employeeCodeProviderMock.Verify(m => m.GetCode(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(3));

        // Verify that the employee salary provider was called for each employee
        employeeSalaryProviderMock.Verify(m => m.GetSalary(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
    }

    [Theory]
    [AutoMoqData]
    public async Task GenerateReportAsync_ShouldReturnFailResult_WhenEmployeeCodeProviderFails(
        [Frozen] Mock<IEmployeeCodeProvider> employeeCodeProviderMock,
        [Frozen] Mock<IEmployeeSalaryProvider> employeeSalaryProviderMock,
        [Frozen] Mock<IEmployeeRepository> employeeRepositoryMock,
        [Frozen] Mock<IMonthNameResolver> monthNameResolverMock,
        Services.ReportService sut)
    {
        // Arrange
        const int year = 2023;
        const int month = 5;
        const string monthName = "May 2023";
        const string employeeInn = "123456789";

        var departmentId = 1L;

        var employee = new EmployeeReportProjection
        {
            Name = "John Doe",
            Inn = employeeInn,
            DepartmentId = departmentId,
            DepartmentName = "IT"
        };

        var employees = new[] {employee};
        var employeeLookup = employees.ToLookup(e => e.DepartmentId);

        monthNameResolverMock
            .Setup(m => m.GetName(year, month))
            .Returns(monthName);

        employeeRepositoryMock
            .Setup(m => m.GetEmployeesForAllActiveDepartmentsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(employeeLookup);

        employeeCodeProviderMock
            .Setup(m => m.GetCode(employeeInn, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail("Failed to get employee code"));

        // Act
        var result = await sut.GenerateReportAsync(year, month, CancellationToken.None);

        // Assert
        result.IsFailed.ShouldBeTrue();
        result.Errors.Should()
            .ContainSingle()
            .Which.Message.Should()
            .Contain(employeeInn);

        // Verify that the employee salary provider was not called
        employeeSalaryProviderMock.Verify(
            m => m.GetSalary(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Theory]
    [AutoMoqData]
    public async Task GenerateReportAsync_ShouldReturnFailResult_WhenEmployeeSalaryProviderFails(
        [Frozen] Mock<IEmployeeCodeProvider> employeeCodeProviderMock,
        [Frozen] Mock<IEmployeeSalaryProvider> employeeSalaryProviderMock,
        [Frozen] Mock<IEmployeeRepository> employeeRepositoryMock,
        [Frozen] Mock<IMonthNameResolver> monthNameResolverMock,
        Services.ReportService sut)
    {
        // Arrange
        const int year = 2023;
        const int month = 5;
        const string monthName = "May 2023";
        const string employeeInn = "123456789";
        const string buhCode = "BUH123";

        var departmentId = 1L;

        var employee = new EmployeeReportProjection
        {
            Name = "John Doe",
            Inn = employeeInn,
            DepartmentId = departmentId,
            DepartmentName = "IT"
        };

        var employees = new[] {employee};
        var employeeLookup = employees.ToLookup(e => e.DepartmentId);

        monthNameResolverMock
            .Setup(m => m.GetName(year, month))
            .Returns(monthName);

        employeeRepositoryMock
            .Setup(m => m.GetEmployeesForAllActiveDepartmentsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(employeeLookup);

        employeeCodeProviderMock
            .Setup(m => m.GetCode(employeeInn, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(buhCode));

        employeeSalaryProviderMock
            .Setup(m => m.GetSalary(employeeInn, buhCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail("Failed to get employee salary"));

        // Act
        var result = await sut.GenerateReportAsync(year, month, CancellationToken.None);

        // Assert
        result.IsFailed.ShouldBeTrue();
        result.Errors.Should()
            .ContainSingle()
            .Which.Message.Should()
            .Contain(employeeInn);
    }

    [Theory]
    [InlineAutoMoqData(0, 0)]
    [InlineAutoMoqData(2023, 5)]
    public async Task GenerateReportAsync_ShouldCallMonthNameResolver_WithCorrectParameters(
        int year,
        int month,
        [Frozen] Mock<IMonthNameResolver> monthNameResolverMock,
        [Frozen] Mock<IEmployeeRepository> employeeRepositoryMock,
        Services.ReportService sut)
    {
        // Arrange
        const string monthName = "Test Month";

        monthNameResolverMock
            .Setup(m => m.GetName(year, month))
            .Returns(monthName);

        employeeRepositoryMock
            .Setup(m => m.GetEmployeesForAllActiveDepartmentsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                Enumerable.Empty<EmployeeReportProjection>()
                    .ToLookup(_ => 0L)
            );

        // Act
        await sut.GenerateReportAsync(year, month, CancellationToken.None);

        // Assert
        monthNameResolverMock.Verify(m => m.GetName(year, month), Times.Once);
    }

    [Theory]
    [AutoMoqData]
    public async Task GenerateReportAsync_ShouldCalculateDepartmentTotalsCorrectly(
        [Frozen] Mock<IEmployeeCodeProvider> employeeCodeProviderMock,
        [Frozen] Mock<IEmployeeSalaryProvider> employeeSalaryProviderMock,
        [Frozen] Mock<IEmployeeRepository> employeeRepositoryMock,
        [Frozen] Mock<IMonthNameResolver> monthNameResolverMock,
        Services.ReportService sut)
    {
        // Arrange
        const int year = 2023;
        const int month = 5;
        const string monthName = "May 2023";

        var departmentId1 = 1L;
        var departmentId2 = 2L;

        var employee1 = new EmployeeReportProjection
        {
            Name = "John Doe",
            Inn = "123456789",
            DepartmentId = departmentId1,
            DepartmentName = "IT"
        };

        var employee2 = new EmployeeReportProjection
        {
            Name = "Jane Smith",
            Inn = "987654321",
            DepartmentId = departmentId1,
            DepartmentName = "IT"
        };

        var employee3 = new EmployeeReportProjection
        {
            Name = "Bob Johnson",
            Inn = "456789123",
            DepartmentId = departmentId2,
            DepartmentName = "Finance"
        };

        var employees = new[] {employee1, employee2, employee3};
        var employeeLookup = employees.ToLookup(e => e.DepartmentId);

        monthNameResolverMock
            .Setup(m => m.GetName(year, month))
            .Returns(monthName);

        employeeRepositoryMock
            .Setup(m => m.GetEmployeesForAllActiveDepartmentsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(employeeLookup);

        employeeCodeProviderMock
            .Setup(m => m.GetCode(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok("BUH123"));

        // Set up different salaries for different employees
        employeeSalaryProviderMock
            .Setup(m => m.GetSalary("123456789", It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(50000));

        employeeSalaryProviderMock
            .Setup(m => m.GetSalary("987654321", It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(60000));

        employeeSalaryProviderMock
            .Setup(m => m.GetSalary("456789123", It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(70000));

        // Act
        var result = await sut.GenerateReportAsync(year, month, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        // The report should contain the correct department totals
        // IT department total: 50000 + 60000 = 110000
        result.Value.Should()
            .Contain("Всего по отделу")
            .And.Contain("110000");

        // Finance department total: 70000
        result.Value.Should()
            .Contain("Всего по отделу")
            .And.Contain("70000");

        // Company total: 110000 + 70000 = 180000
        result.Value.Should()
            .Contain("Всего по предприятию")
            .And.Contain("180000");
    }
}