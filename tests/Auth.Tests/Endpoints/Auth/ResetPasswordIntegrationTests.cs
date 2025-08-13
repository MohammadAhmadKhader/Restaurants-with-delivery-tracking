using Auth.Contracts.Dtos.Auth;
using Auth.Tests.Collections;
using Auth.Utils;
using Shared.Validation.FluentValidation;

namespace Auth.Tests.Endpoints.Auth;

[Collection("IntegrationTests")]
public class ResetPasswordIntegrationTests(IntegrationTestsFixture fixture, ITestOutputHelper output)
{
    private readonly IntegrationTestsFixture _fixture = fixture;
    private readonly ITestOutputHelper _out = output;
    private readonly string _endpoint = "api/auth/reset-password";
    private readonly HttpClient _client = fixture.CreateClientWithTestOutput(output);

    public static IEnumerable<object[]> InvalidResetPasswordInputs()
    {
        var expectedStatus = HttpStatusCode.BadRequest;
        var shortPassword = new string('a', Constants.MinPasswordLength - 1);
        var longPassword = new string('x', Constants.MaxPasswordLength + 1);
        const string oldPassword = "oldPassword";
        const string newPassword = "newPassword";
        const string confirmNewPassword = "confirmNewPassword";

        return
        [
            // oldPassword
            [
                new { newPassword = "newPassword123", confirmNewPassword = "newPassword123" },
                expectedStatus, oldPassword,
                ValidationMessagesBuilder.Required(nameof(ResetPasswordDto.OldPassword))
            ],
            [
                new { oldPassword = shortPassword, newPassword = "newPassword123", confirmNewPassword = "newPassword123" },
                expectedStatus, oldPassword,
                PasswordLengthBetweenFormatter(nameof(ResetPasswordDto.OldPassword))
            ],
            [
                new { oldPassword = longPassword, newPassword = "newPassword123", confirmNewPassword = "newPassword123" },
                expectedStatus, oldPassword,
                PasswordLengthBetweenFormatter(nameof(ResetPasswordDto.OldPassword))
            ],
            // newPassword
            [
                new { oldPassword = "oldPassword123", confirmNewPassword = "newPassword123" },
                expectedStatus, newPassword,
                ValidationMessagesBuilder.Required(nameof(ResetPasswordDto.NewPassword))
            ],
            [
                new { oldPassword = "oldPassword123", newPassword = shortPassword, confirmNewPassword = shortPassword },
                expectedStatus, newPassword,
                PasswordLengthBetweenFormatter(nameof(ResetPasswordDto.NewPassword))
            ],
            [
                new { oldPassword = "oldPassword123", newPassword = longPassword, confirmNewPassword = longPassword },
                expectedStatus, newPassword,
                PasswordLengthBetweenFormatter(nameof(ResetPasswordDto.NewPassword))
            ],
            // confirmNewPassword
            [
                new { oldPassword = "oldPassword123", newPassword = "newPassword123" },
                expectedStatus, confirmNewPassword,
                ValidationMessagesBuilder.Required(nameof(ResetPasswordDto.ConfirmNewPassword))
            ],
            [
                new { oldPassword = "oldPassword123", newPassword = "newPassword123", confirmNewPassword = shortPassword },
                expectedStatus, confirmNewPassword,
                PasswordLengthBetweenFormatter(nameof(ResetPasswordDto.ConfirmNewPassword))
            ],
            [
                new { oldPassword = "oldPassword123", newPassword = longPassword, confirmNewPassword = longPassword },
                expectedStatus, confirmNewPassword,
                PasswordLengthBetweenFormatter(nameof(ResetPasswordDto.ConfirmNewPassword))
            ],
            [
                new { oldPassword = "oldPassword123", newPassword = "newPassword123", confirmNewPassword = "differentPassword123" },
                expectedStatus, confirmNewPassword, "Passwords mismatch."
            ]
        ];
    }

    [Theory]
    [MemberData(nameof(InvalidResetPasswordInputs))]
    public async Task ResetPassword_InvalidInputs_ReturnsValidationError(object payload, HttpStatusCode expectedStatus, string field, string expectedMessage)
    {
        TestUtils.LogPayload(_out, [new { payload, expectedStatus, field, expectedMessage}]);
        var user = _fixture.Loader.Users.ElementAtOrDefault(0);
        Assert.NotNull(user);

        var response = await TestUtils.SendWithAuthAsync(_client, HttpMethod.Post, user.Email!,
         _fixture.TestPassword, _endpoint, JsonContent.Create(payload));

        Assert.Equal(expectedStatus, response.StatusCode);

        await TestUtils.AssertValidationError(response, field, expectedMessage);
    }
    
    private static string PasswordLengthBetweenFormatter(string fieldName) =>
        ValidationMessagesBuilder.LengthBetween(fieldName, Constants.MinPasswordLength, Constants.MaxPasswordLength);
}