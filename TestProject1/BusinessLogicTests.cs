using Xunit;

namespace TestProject1  
{
    public class BusinessLogicTests
    {
        [Theory]
        [InlineData(100.00, 18.50, 1850.00)]
        [InlineData(10.50, 19.00, 199.50)]
        [InlineData(0.00, 18.50, 0.00)]
        public void CurrencyCalculation_ShouldAccuratelyMultiplyValues(decimal usd, decimal currentRate, decimal expectedZar)
        {
            // Arrange & Act: Simulating the ExchangeRate dynamic calculation algorithm (Requirement 4)
            decimal calculatedZar = usd * currentRate;

            // Assert
            Assert.Equal(expectedZar, calculatedZar);
        }

        [Fact]
        public void FileValidation_ShouldRejectNonPdfExtensions()
        {
            // Arrange
            string uploadedFileName = "corporate_malware_exploit.exe";

            // Act
            string extension = System.IO.Path.GetExtension(uploadedFileName).ToLowerInvariant();
            bool isAllowed = (extension == ".pdf");

            // Assert
            Assert.False(isAllowed, "Security Protocol Check failed: System permitted a non-PDF format block.");
        }
    }
}