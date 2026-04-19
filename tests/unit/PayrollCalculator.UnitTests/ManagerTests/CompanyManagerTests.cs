using NSubstitute;
using PayrollCalculator.Domain.Models;
using PayrollCalculator.Managers;
using PayrollCalculator.Repositories.Contracts;
using PayrollCalculator.UnitTests.Builders;
using PayrollCalculator.Utilities.Contracts;

namespace PayrollCalculator.UnitTests.ManagerTests;

[TestFixture]
public class CompanyManagerTests
{
    private ICompanyRepository _companyRepository = null!;
    private IWideEventContext  _wideEvent         = null!;
    private CompanyManager     _manager           = null!;

    [SetUp]
    public void SetUp()
    {
        _companyRepository = Substitute.For<ICompanyRepository>();
        _wideEvent         = Substitute.For<IWideEventContext>();
        _manager           = new CompanyManager(_companyRepository, _wideEvent);
    }

    [Test]
    public async Task Given_ExistingCompanyId_When_GetCompanyAsync_Then_CompanyIsReturned()
    {
        // Given
        var company = new CompanyBuilder().WithId(42).Build();
        _companyRepository.GetByIdAsync(42).Returns(company);

        // When
        var result = await _manager.GetCompanyAsync(42);

        // Then
        Assert.That(result, Is.SameAs(company));
    }

    [Test]
    public async Task Given_NonExistingCompanyId_When_GetCompanyAsync_Then_NullIsReturned()
    {
        // Given
        _companyRepository.GetByIdAsync(Arg.Any<int>()).Returns((Company?)null);

        // When
        var result = await _manager.GetCompanyAsync(99);

        // Then
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Given_Companies_When_GetAllCompaniesAsync_Then_AllCompaniesAreReturned()
    {
        // Given
        var companies = new[] { new CompanyBuilder().WithId(1).Build(), new CompanyBuilder().WithId(2).Build() };
        _companyRepository.GetAllAsync().Returns(companies);

        // When
        var result = await _manager.GetAllCompaniesAsync();

        // Then
        Assert.That(result, Is.EquivalentTo(companies));
    }

    [Test]
    public async Task Given_NewCompany_When_CreateCompanyAsync_Then_NewIdIsReturned()
    {
        // Given
        var company = new CompanyBuilder().Build();
        _companyRepository.CreateAsync(company).Returns(7);

        // When
        var id = await _manager.CreateCompanyAsync(company);

        // Then
        Assert.That(id, Is.EqualTo(7));
    }

    [Test]
    public async Task Given_NewCompany_When_CreateCompanyAsync_Then_RepositoryCreateIsInvoked()
    {
        // Given
        var company = new CompanyBuilder().Build();
        _companyRepository.CreateAsync(company).Returns(1);

        // When
        await _manager.CreateCompanyAsync(company);

        // Then
        await _companyRepository.Received(1).CreateAsync(company);
    }

    [Test]
    public async Task Given_ExistingCompany_When_UpdateCompanyAsync_Then_RepositoryUpdateIsInvoked()
    {
        // Given
        var company = new CompanyBuilder().Build();

        // When
        await _manager.UpdateCompanyAsync(company);

        // Then
        await _companyRepository.Received(1).UpdateAsync(company);
    }

    [Test]
    public async Task Given_ExistingCompanyId_When_DeleteCompanyAsync_Then_RepositoryDeleteIsInvoked()
    {
        // Given
        const int companyId = 5;

        // When
        await _manager.DeleteCompanyAsync(companyId);

        // Then
        await _companyRepository.Received(1).DeleteAsync(companyId);
    }
}
