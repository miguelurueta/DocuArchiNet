using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Web;
using Login2Fa = GestionDocumental_Docuarchi.net;

internal static class LoginSecondFactorFoundationBehaviorTests
{
    private static int Main()
    {
        try
        {
            IdentityAndConfigurationEnforceTheContract();
            ChallengeUsesTheInjectedClock();
            OtpUsesRejectionSamplingAndSixDigits();
            HmacIsVersionedBoundAndRotatable();
            KeyConfigurationFailsClosed();
            PendingSessionContextIsMinimalAndExpires();
            OrchestrationContractsRemainInternalAndTyped();
            WebFormsAdaptersRemainNarrowAndSanitized();
            PublicDtosRemainSanitized();
            Console.WriteLine("login-second-factor foundation behavior tests: passed");
            return 0;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine(error.ToString());
            return 1;
        }
    }

    private static void IdentityAndConfigurationEnforceTheContract()
    {
        var identity = new Login2Fa.SegundoFactorIdentity(7, 11, " radicador ", "42", " luz.aguilera ");
        Equal("RADICADOR", identity.TipoUsuario, "tipo normalizado");
        Equal("LUZ.AGUILERA", identity.LoginNormalizado, "login normalizado");
        Equal("7:11:RADICADOR:42", identity.ClaveCanonica, "identidad canónica");

        Assert(!new Login2Fa.SegundoFactorConfiguration(null, null, null).IsRequired, "NULL debe preservar login legacy.");
        Assert(!new Login2Fa.SegundoFactorConfiguration(0, 99, 99).IsRequired, "Cero debe preservar login legacy.");
        var active = new Login2Fa.SegundoFactorConfiguration(1, 1, 5);
        Assert(active.IsRequired && active.ProviderType == 1 && active.ExpirationMinutes == 5, "Configuración EMAIL válida.");
        Throws<ArgumentOutOfRangeException>(() => new Login2Fa.SegundoFactorConfiguration(2, 1, 5), "flag inválido");
        Throws<ArgumentOutOfRangeException>(() => new Login2Fa.SegundoFactorConfiguration(1, 2, 5), "TOTP no soportado");
        Throws<ArgumentOutOfRangeException>(() => new Login2Fa.SegundoFactorConfiguration(1, 1, 0), "expiración menor");
        Throws<ArgumentOutOfRangeException>(() => new Login2Fa.SegundoFactorConfiguration(1, 1, 11), "expiración mayor");

        var stateNames = Enum.GetNames(typeof(Login2Fa.SegundoFactorChallengeState));
        Equal(9, stateNames.Length, "cantidad de estados");
        Assert(stateNames.SequenceEqual(new[] { "CREATED", "SENT", "FINALIZING", "COMPLETED", "DELIVERY_FAILED", "BLOCKED", "EXPIRED", "REVOKED", "FINALIZATION_FAILED" }), "Estados vinculantes.");
        Equal(5, Login2Fa.SegundoFactorConfiguration.MaxAttempts, "intentos");
        Equal(60, Login2Fa.SegundoFactorConfiguration.ResendCooldownSeconds, "cooldown");
        Equal(2, Login2Fa.SegundoFactorConfiguration.MaxResends, "reenvíos");
        Equal("l***@example.test", Login2Fa.SecondFactorContractValidation.MaskEmailAddress("luz.aguilera@example.test"), "correo enmascarado");
        Equal("*@example.test", Login2Fa.SecondFactorContractValidation.MaskEmailAddress("a@example.test"), "correo corto enmascarado");
        Assert(Login2Fa.SecondFactorContractValidation.IsMaskedEmailDestination("u***@example.test"), "destino enmascarado válido");
        Assert(!Login2Fa.SecondFactorContractValidation.IsMaskedEmailDestination("user@example.test*"), "el asterisco fuera de la parte local no enmascara el destino");
        Throws<ArgumentException>(() => new Login2Fa.SecondFactorRecipient("user@example.test", "user@example.test*"), "destino falsamente enmascarado");
        Throws<ArgumentException>(() => Login2Fa.SecondFactorContractValidation.MaskEmailAddress("invalid"), "correo inválido");
    }

    private static void ChallengeUsesTheInjectedClock()
    {
        var now = new DateTime(2026, 10, 9, 12, 0, 0, DateTimeKind.Utc);
        var identity = new Login2Fa.SegundoFactorIdentity(1, 2, "GESTION", "3", "USER");
        var challenge = new Login2Fa.SegundoFactorChallenge(Guid.NewGuid(), identity, Login2Fa.SegundoFactorPurpose.LOGIN,
            Login2Fa.SegundoFactorChallengeState.SENT, 0, 0, now, now.AddMinutes(5));
        var clock = new FakeClock { Value = now.AddMinutes(4) };
        Assert(!challenge.IsExpired(clock), "No debe expirar antes del límite.");
        clock.Value = now.AddMinutes(5);
        Assert(challenge.IsExpired(clock), "Debe expirar exactamente en el límite.");
    }

    private static void OtpUsesRejectionSamplingAndSixDigits()
    {
        var random = new SequenceRandomNumberGenerator(4294000000U, 42U);
        using (var generator = new Login2Fa.CryptographicSecondFactorOtpGenerator(random))
        {
            Equal("000042", generator.GenerateCode(), "OTP con ceros iniciales");
            Equal(2, random.Calls, "la muestra fuera del límite debe descartarse");
        }
    }

    private static void HmacIsVersionedBoundAndRotatable()
    {
        var settings = new DictionarySettings();
        settings.Values[Login2Fa.AppSettingsSecondFactorKeyProvider.ActiveKeyIdSetting] = "old";
        settings.Values[Login2Fa.AppSettingsSecondFactorKeyProvider.KeySettingPrefix + "old"] = Convert.ToBase64String(Enumerable.Repeat((byte)0x31, 32).ToArray());
        settings.Values[Login2Fa.AppSettingsSecondFactorKeyProvider.KeySettingPrefix + "new"] = Convert.ToBase64String(Enumerable.Repeat((byte)0x72, 32).ToArray());
        var protector = new Login2Fa.HmacSecondFactorCodeProtector(new Login2Fa.AppSettingsSecondFactorKeyProvider(settings));
        var id = Guid.NewGuid();
        var context = new Login2Fa.SecondFactorProtectionContext(Login2Fa.SegundoFactorPurpose.LOGIN, id, "1:2:GESTION:3", "session-hash-a");
        var protectedCode = protector.Protect(context, "123456");
        Assert(protectedCode.StartsWith("v1:old:", StringComparison.Ordinal), "Formato y keyId.");
        Assert(protector.Verify(context, "123456", protectedCode), "Round-trip HMAC.");

        Assert(!protector.Verify(context, "123457", protectedCode), "Código alterado.");
        Assert(!protector.Verify(new Login2Fa.SecondFactorProtectionContext(Login2Fa.SegundoFactorPurpose.LOGIN, Guid.NewGuid(), context.CanonicalIdentity, context.SessionBinding), "123456", protectedCode), "Challenge alterado.");
        Assert(!protector.Verify(new Login2Fa.SecondFactorProtectionContext(Login2Fa.SegundoFactorPurpose.LOGIN, id, "1:2:GESTION:4", context.SessionBinding), "123456", protectedCode), "Identidad alterada.");
        Assert(!protector.Verify(new Login2Fa.SecondFactorProtectionContext(Login2Fa.SegundoFactorPurpose.LOGIN, id, context.CanonicalIdentity, "session-hash-b"), "123456", protectedCode), "Sesión alterada.");
        Throws<InvalidOperationException>(() => protector.Verify(context, "123456", "v2:old:AAAA"), "versión desconocida");
        Throws<InvalidOperationException>(() => protector.Verify(context, "123456", "v1:missing:AAAA"), "llave desconocida");
        Throws<InvalidOperationException>(() => protector.Verify(context, "123456", "v1:old:not-base64"), "Base64 inválido");
        Assert(!protector.Verify(context, "123456", "v1:old:" + Convert.ToBase64String(new byte[31])), "Longitud distinta.");

        settings.Values[Login2Fa.AppSettingsSecondFactorKeyProvider.ActiveKeyIdSetting] = "new";
        var newProtectedCode = protector.Protect(context, "654321");
        Assert(newProtectedCode.StartsWith("v1:new:", StringComparison.Ordinal), "Nueva protección usa llave activa.");
        Assert(protector.Verify(context, "123456", protectedCode), "Challenge anterior verifica durante rotación.");
    }

    private static void KeyConfigurationFailsClosed()
    {
        var settings = new DictionarySettings();
        var provider = new Login2Fa.AppSettingsSecondFactorKeyProvider(settings);
        Throws<InvalidOperationException>(() => provider.GetActiveKey(), "activa ausente");
        settings.Values[Login2Fa.AppSettingsSecondFactorKeyProvider.ActiveKeyIdSetting] = "bad";
        settings.Values[Login2Fa.AppSettingsSecondFactorKeyProvider.KeySettingPrefix + "bad"] = "not-base64";
        Throws<InvalidOperationException>(() => provider.GetActiveKey(), "Base64 inválido");
        settings.Values[Login2Fa.AppSettingsSecondFactorKeyProvider.KeySettingPrefix + "bad"] = Convert.ToBase64String(new byte[31]);
        Throws<InvalidOperationException>(() => provider.GetActiveKey(), "llave corta");
    }

    private static void PendingSessionContextIsMinimalAndExpires()
    {
        var now = new DateTime(2026, 10, 9, 15, 0, 0, DateTimeKind.Utc);
        var clock = new FakeClock { Value = now };
        var session = new DictionarySession();
        var store = new Login2Fa.SessionPendingSecondFactorContextStore(session, clock);
        var pending = new Login2Fa.PendingSecondFactorContext(1, 2, 3, " user ", "u***@example.test", Guid.NewGuid(), "synthetic-nonce", now, now.AddMinutes(5));
        store.Save(pending);
        Equal("USER", store.GetCurrent().NormalizedLogin, "round-trip Session");
        Equal(String.Empty, store.GetCurrent().ModuleType, "constructor compatible sin tipo de módulo");
        Equal(0, store.GetCurrent().ResendCount, "constructor compatible sin reenvíos");
        store.Clear();
        Assert(store.GetCurrent() == null, "Clear explícito elimina el contexto.");
        Equal(0, session.Values.Count, "Session limpia después de Clear.");

        store.Save(pending);
        clock.Value = now.AddMinutes(5);
        Assert(store.GetCurrent() == null, "El contexto expirado se elimina.");
        Equal(0, session.Values.Count, "Session limpia al expirar.");

        var complete = new Login2Fa.PendingSecondFactorContext(1, 2, " RADICACION DOCUMENTAL ", 3, " user ", "u***@example.test", Guid.NewGuid(), "new-nonce", 2, now, now.AddMinutes(5));
        Equal("RADICACION DOCUMENTAL", complete.ModuleType, "tipo de módulo normalizado");
        Equal(2, complete.ResendCount, "contador de reenvíos");
        Throws<ArgumentOutOfRangeException>(() => new Login2Fa.PendingSecondFactorContext(1, 2, "RADICACION DOCUMENTAL", 3, "user", "u***@example.test", Guid.NewGuid(), "nonce", 3, now, now.AddMinutes(5)), "límite de reenvíos");

        var forbidden = new[] { "Password", "Otp", "Code", "Email", "Hmac", "Key", "Connection", "Credential" };
        var names = typeof(Login2Fa.PendingSecondFactorContext).GetProperties().Select(p => p.Name).ToArray();
        Assert(!names.Any(name => forbidden.Any(word => name.IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0)), "El contexto no debe declarar secretos.");
    }

    private static void PublicDtosRemainSanitized()
    {
        Assert(typeof(Login2Fa.SegundoFactorEstadoDto).IsSerializable, "Estado DTO serializable.");
        Assert(typeof(Login2Fa.SegundoFactorResultadoDto).IsSerializable, "Resultado DTO serializable.");
        var stateNames = typeof(Login2Fa.SegundoFactorEstadoDto).GetProperties().Select(p => p.Name).OrderBy(x => x).ToArray();
        Assert(stateNames.SequenceEqual(new[] { "DestinoEnmascarado", "EstadoPublico", "ExpiraEnSegundos", "ReenvioDisponibleEnSegundos", "RequiereCodigo" }.OrderBy(x => x)), "Superficie exacta de estado.");
        var resultNames = typeof(Login2Fa.SegundoFactorResultadoDto).GetProperties().Select(p => p.Name).OrderBy(x => x).ToArray();
        Assert(resultNames.SequenceEqual(new[] { "Codigo", "Estado", "Exito", "MensajeVisible", "RutaLocal" }.OrderBy(x => x)), "Superficie exacta de resultado.");
        var forbidden = new[] { "Challenge", "Id", "Login", "Email", "Nonce", "Hmac", "Hash", "Exception", "Sql", "Smtp", "ReturnUrl" };
        Assert(!stateNames.Concat(resultNames).Any(name => forbidden.Any(word => name.IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0)), "Los DTO no deben declarar datos internos.");
    }

    private static void OrchestrationContractsRemainInternalAndTyped()
    {
        var service = typeof(Login2Fa.ISecondFactorAuthenticationService);
        var signatures = service.GetMethods().ToDictionary(method => method.Name, method => method);
        Assert(signatures.Keys.OrderBy(x => x).SequenceEqual(new[] { "Cancel", "GetStatus", "Resend", "Start", "Verify" }.OrderBy(x => x)), "El servicio expone exactamente cinco casos de uso.");
        Equal(typeof(Login2Fa.SecondFactorAuthenticationStartRequest), signatures["Start"].GetParameters().Single().ParameterType, "request tipado de inicio");
        Equal(typeof(string), signatures["Verify"].GetParameters().Single().ParameterType, "código tipado");
        Assert(signatures.Values.All(method => method.ReturnType == typeof(Login2Fa.SecondFactorAuthenticationResult)), "Todos los casos retornan el resultado interno.");

        var result = new Login2Fa.SecondFactorAuthenticationResult(false, " rejected ", Login2Fa.SecondFactorAuthenticationState.REJECTED, String.Empty, 0, 0, String.Empty);
        Equal("REJECTED", result.PublicCode, "código público normalizado");
        var forbidden = new[] { "Password", "Otp", "Challenge", "Nonce", "Email", "Hmac", "Hash", "Connection", "Credential" };
        foreach (var type in new[] { typeof(Login2Fa.SecondFactorAuthenticationStartRequest), typeof(Login2Fa.SecondFactorAuthenticationResult) })
            Assert(!type.GetProperties().Any(property => forbidden.Any(word => property.Name.IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0)), type.Name + " no declara secretos.");

        var ticketMethod = typeof(Login2Fa.ISecondFactorAuthenticationTicketIssuer).GetMethod("Issue");
        Equal(typeof(void), ticketMethod.ReturnType, "emisor de ticket");
        Equal(typeof(string), ticketMethod.GetParameters().Single().ParameterType, "login para ticket");
        Equal(typeof(string), typeof(Login2Fa.ILoginLocalRouteResolver).GetMethod("Resolve").ReturnType, "resolvedor de ruta local");
    }

    private static void WebFormsAdaptersRemainNarrowAndSanitized()
    {
        var now = new DateTime(2026, 10, 10, 12, 0, 0, DateTimeKind.Utc);
        var clock = new FakeClock { Value = now };
        var session = new DictionarySession();
        var store = new Login2Fa.SessionPendingSecondFactorContextStore(session, clock);
        store.Save(new Login2Fa.PendingSecondFactorContext(1, 2, "RADICACION DOCUMENTAL", 3, "USER", "u***@example.test", Guid.NewGuid(), "nonce", 0, now, now.AddMinutes(5)));
        var gate = new Login2Fa.LoginSecondFactorSessionGate(new FakeHttpContext(session), clock);
        Equal("RADICACION DOCUMENTAL", gate.AsegurarContexto().ModuleType, "gate obtiene solo Session");
        clock.Value = now.AddMinutes(5);
        Assert(gate.AsegurarContexto() == null, "gate rechaza expirado");

        string issuedLogin = null; bool persistent = true;
        var issuer = new Login2Fa.FormsAuthenticationTicketIssuer((login, remember) => { issuedLogin = login; persistent = remember; });
        issuer.Issue(" user ");
        Equal("USER", issuedLogin, "ticket normalizado");
        Equal(false, persistent, "cookie no persistente");
        Equal("~/Defaul/WebFormInicioDocuarchiGestion.aspx", new Login2Fa.LegacyLoginLocalRouteResolver().Resolve(), "ruta local fija");

        var mapper = new Login2Fa.LoginSecondFactorPresentationMapper();
        var pending = mapper.MapResult(new Login2Fa.SecondFactorAuthenticationResult(false, "SECOND_FACTOR_PENDING", Login2Fa.SecondFactorAuthenticationState.PENDING, "u***@example.test", 120, 30, String.Empty));
        Equal(true, pending.Estado.RequiereCodigo, "estado pendiente");
        Equal(String.Empty, pending.RutaLocal, "sin ruta antes de completar");
        var completed = mapper.MapResult(new Login2Fa.SecondFactorAuthenticationResult(true, "SECOND_FACTOR_COMPLETED", Login2Fa.SecondFactorAuthenticationState.COMPLETED, String.Empty, 0, 0, "~/Defaul/WebFormInicioDocuarchiGestion.aspx"));
        Equal("~/Defaul/WebFormInicioDocuarchiGestion.aspx", completed.RutaLocal, "ruta solo al completar");
        var internalFailure = mapper.Failure("internal_sql_error");
        Equal("SECOND_FACTOR_UNAVAILABLE", internalFailure.Codigo, "código interno fuera del catálogo");
        Assert(!internalFailure.MensajeVisible.Contains("sql"), "error interno saneado");
        Assert(mapper.Failure("SECOND_FACTOR_DELIVERY_FAILED").MensajeVisible.Contains("enviar"), "descripción segura de entrega");
        Assert(mapper.Failure("SECOND_FACTOR_BLOCKED").MensajeVisible.Contains("intentos"), "descripción segura de bloqueo");
    }

    private static void Assert(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    private static void Equal<T>(T expected, T actual, string label)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
            throw new InvalidOperationException(label + ": esperado=" + expected + ", actual=" + actual);
    }

    private static void Throws<T>(Action action, string label) where T : Exception
    {
        try { action(); }
        catch (T) { return; }
        throw new InvalidOperationException(label + ": no lanzó " + typeof(T).Name);
    }

    private sealed class FakeClock : Login2Fa.ISecondFactorClock
    {
        public DateTime Value { get; set; }
        public DateTime UtcNow { get { return Value; } }
    }

    private sealed class DictionarySettings : Login2Fa.ISecondFactorSettings
    {
        public readonly Dictionary<string, string> Values = new Dictionary<string, string>(StringComparer.Ordinal);
        public string GetValue(string name) { string value; return Values.TryGetValue(name, out value) ? value : null; }
    }

    private sealed class SequenceRandomNumberGenerator : RandomNumberGenerator
    {
        private readonly Queue<uint> _samples;
        public int Calls { get; private set; }
        public SequenceRandomNumberGenerator(params uint[] samples) { _samples = new Queue<uint>(samples); }
        public override void GetBytes(byte[] data)
        {
            Calls++;
            var bytes = BitConverter.GetBytes(_samples.Dequeue());
            Buffer.BlockCopy(bytes, 0, data, 0, 4);
        }
    }

    private sealed class DictionarySession : HttpSessionStateBase
    {
        public readonly Dictionary<string, object> Values = new Dictionary<string, object>(StringComparer.Ordinal);
        public override object this[string name]
        {
            get { object value; return Values.TryGetValue(name, out value) ? value : null; }
            set { Values[name] = value; }
        }
        public override void Remove(string name) { Values.Remove(name); }
    }
    private sealed class FakeHttpContext : HttpContextBase
    {
        private readonly HttpSessionStateBase _session;
        public FakeHttpContext(HttpSessionStateBase session) { _session = session; }
        public override HttpSessionStateBase Session { get { return _session; } }
    }
}
