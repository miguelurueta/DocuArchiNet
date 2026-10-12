using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Login2Fa = GestionDocumental_Docuarchi.net;

internal static class LoginSecondFactorAuthenticationBehaviorTests
{
    private static int Main()
    {
        try
        {
            StartStatusAndVerifyCompleteInOrder();
            DeliveryAndStatusFailuresCloseSafely();
            CreationConfirmationAndFinalizationFailuresNeverIssueTicket();
            DiagnosticsAreCorrelatedAndNonBlocking();
            ProtectionFailureDoesNotConsumeAnOtpAttempt();
            InvalidFormatDoesNotReadRepository();
            FifthAttemptBlocksAndConcurrentVerifyHasOneWinner();
            SessionAndIdentityIsolationRejectCrossContext();
            ResendAndCancelReplaceAndRevoke();
            Console.WriteLine("login-second-factor authentication behavior tests: passed");
            return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }

    private static void StartStatusAndVerifyCompleteInOrder()
    {
        var h = new Harness();
        var started = h.Service.Start(h.Request());
        Equal(Login2Fa.SecondFactorAuthenticationState.PENDING, started.State, "inicio pendiente");
        Equal("SECOND_FACTOR_REQUIRED", started.PublicCode, "código inicio");
        Equal("RADICACION DOCUMENTAL", h.Store.Value.ModuleType, "módulo en Session");
        Equal(1, h.Repository.CreateCalls, "creación");
        Equal(1, h.Email.Calls, "correo");
        Equal(1, h.Repository.MarkSentCalls, "confirmación envío");
        Equal("SECOND_FACTOR_PENDING", h.Service.GetStatus().PublicCode, "estado");

        var completed = h.Service.Verify("123456");
        Equal(true, completed.Success, "éxito");
        Equal("~/Defaul/WebFormInicioDocuarchiGestion.aspx", completed.LocalRoute, "ruta fija");
        Equal(1, h.Finalizer.Calls, "finalizador único");
        Equal(1, h.Repository.BeginCalls, "adquisición única");
        Equal(1, h.Repository.CompleteCalls, "confirmación antes de ticket");
        Equal(1, h.Tickets.Calls, "ticket único");
        Equal(null, h.Store.Value, "Session limpia");
        Equal("complete,ticket", String.Join(",", h.Order), "orden de cierre");
    }

    private static void InvalidFormatDoesNotReadRepository()
    {
        var h = new Harness();
        h.Service.Start(h.Request());
        var result = h.Service.Verify("12A");
        Equal("SECOND_FACTOR_CODE_INVALID", result.PublicCode, "formato inválido");
        Equal(0, h.Repository.GetCalls, "sin lectura de repositorio");
        Equal(0, h.Finalizer.Calls, "sin finalización");
        Equal(0, h.Tickets.Calls, "sin cookie");
    }

    private static void ProtectionFailureDoesNotConsumeAnOtpAttempt()
    {
        var h = new Harness();
        h.Service.Start(h.Request());
        h.Protector.ThrowOnVerify = true;
        Equal("SECOND_FACTOR_VERIFICATION_FAILED", h.Service.Verify("123456").PublicCode, "falla interna de protección");
        Equal(0, h.Repository.Challenge.Attempts, "la falla de llave no consume intento");
        Assert(h.Diagnostics.Contains(Login2Fa.SecondFactorDiagnosticStage.CODE_VERIFICATION, Login2Fa.SecondFactorDiagnosticOutcome.FAILED), "falla HMAC localizada");
    }

    private static void DeliveryAndStatusFailuresCloseSafely()
    {
        var delivery = new Harness();
        delivery.Email.Success = false;
        Equal("SECOND_FACTOR_DELIVERY_FAILED", delivery.Service.Start(delivery.Request()).PublicCode, "fallo de entrega");
        Equal(true, delivery.Repository.Consumed, "challenge de entrega consumido");
        Equal(null, delivery.Store.Value, "sin Session tras fallo");
        Equal(0, delivery.Tickets.Calls, "sin cookie tras fallo");

        var status = new Harness();
        Equal("SECOND_FACTOR_SESSION_INVALID", status.Service.GetStatus().PublicCode, "Session ausente");
        status.Service.Start(status.Request());
        status.Clock.Value = status.Clock.Value.AddMinutes(5);
        Equal(0, status.Service.GetStatus().ExpiresInSeconds, "límite de expiración");
    }

    private static void FifthAttemptBlocksAndConcurrentVerifyHasOneWinner()
    {
        var attempts = new Harness();
        attempts.Service.Start(attempts.Request());
        for (var i = 1; i < 5; i++) Equal("SECOND_FACTOR_CODE_INVALID", attempts.Service.Verify("000000").PublicCode, "intento " + i);
        Equal("SECOND_FACTOR_BLOCKED", attempts.Service.Verify("000000").PublicCode, "quinto intento");
        Equal(null, attempts.Store.Value, "Session limpia al bloquear");

        var race = new Harness();
        race.Service.Start(race.Request());
        var otherStore = new Store { Value = race.Store.Value };
        var otherService = race.CreateService(otherStore);
        var tasks = new[] { Task.Run(() => race.Service.Verify("123456")), Task.Run(() => otherService.Verify("123456")) };
        Task.WaitAll(tasks);
        Equal(1, tasks.Count(task => task.Result.Success), "un ganador concurrente");
        Equal(1, race.Finalizer.Calls, "un finalizador concurrente");
        Equal(1, race.Tickets.Calls, "una cookie concurrente");
    }

    private static void CreationConfirmationAndFinalizationFailuresNeverIssueTicket()
    {
        var create = new Harness(); create.Repository.CreateSuccess = false;
        Equal("SECOND_FACTOR_DELIVERY_FAILED", create.Service.Start(create.Request()).PublicCode, "fallo de creación");
        Equal(0, create.Tickets.Calls, "creación sin cookie");

        var sent = new Harness(); sent.Repository.MarkSentSuccess = false;
        Equal("SECOND_FACTOR_DELIVERY_FAILED", sent.Service.Start(sent.Request()).PublicCode, "fallo de confirmación");
        Equal(true, sent.Repository.Consumed, "confirmación fallida consume");

        var finalizer = new Harness(); finalizer.Service.Start(finalizer.Request()); finalizer.Finalizer.Success = false;
        Equal("LOGIN_FINALIZATION_FAILED", finalizer.Service.Verify("123456").PublicCode, "fallo finalizador");
        Equal(0, finalizer.Tickets.Calls, "finalizador sin cookie");

        var complete = new Harness(); complete.Service.Start(complete.Request()); complete.Repository.CompleteSuccess = false;
        Equal("LOGIN_FINALIZATION_FAILED", complete.Service.Verify("123456").PublicCode, "fallo Complete");
        Equal(0, complete.Tickets.Calls, "Complete sin cookie");

        var ticket = new Harness(); ticket.Service.Start(ticket.Request()); ticket.Tickets.ThrowOnIssue = true;
        Equal("SECOND_FACTOR_VERIFICATION_FAILED", ticket.Service.Verify("123456").PublicCode, "fallo ticket");
        Equal(null, ticket.Store.Value, "fallo ticket limpia Session");
    }

    private static void SessionAndIdentityIsolationRejectCrossContext()
    {
        var h = new Harness();
        h.Service.Start(h.Request());
        var original = h.Store.Value;

        h.Store.Value = new Login2Fa.PendingSecondFactorContext(
            original.EmpresaId + 1, original.ModuloId, original.ModuleType,
            original.InternalUserId, original.NormalizedLogin, original.MaskedDestination,
            original.ChallengeId, "different-session-binding", original.ResendCount,
            original.CreatedAtUtc, original.ExpiresAtUtc);
        Equal("SECOND_FACTOR_SESSION_INVALID", h.Service.Verify("123456").PublicCode, "binding de otra sesión");
        Equal(0, h.Finalizer.Calls, "otra sesión no finaliza");
        Equal(0, h.Tickets.Calls, "otra sesión no obtiene cookie");

        foreach (var altered in new[] {
            AlterContext(original, original.EmpresaId + 1, original.ModuloId, original.ModuleType, original.InternalUserId, original.NormalizedLogin),
            AlterContext(original, original.EmpresaId, original.ModuloId + 1, original.ModuleType, original.InternalUserId, original.NormalizedLogin),
            AlterContext(original, original.EmpresaId, original.ModuloId, "GESTOR DOCUMENTAL", original.InternalUserId, original.NormalizedLogin),
            AlterContext(original, original.EmpresaId, original.ModuloId, original.ModuleType, original.InternalUserId + 1, original.NormalizedLogin)
        })
        {
            var isolatedStore = new Store { Value = altered };
            Equal("SECOND_FACTOR_SESSION_INVALID", h.CreateService(isolatedStore).Verify("123456").PublicCode, "identidad cruzada");
        }
        Equal(0, h.Finalizer.Calls, "identidades cruzadas no finalizan");
        Equal(0, h.Tickets.Calls, "identidades cruzadas no obtienen cookie");

        var replayStore = new Store { Value = original };
        var replayService = h.CreateService(replayStore);
        Equal("SECOND_FACTOR_COMPLETED", replayService.Verify("123456").PublicCode, "sesión autoritativa completa");
        replayStore.Value = original;
        Equal("SECOND_FACTOR_SESSION_INVALID", replayService.Verify("123456").PublicCode, "challenge consumido no admite replay");
        Equal(1, h.Finalizer.Calls, "replay no repite finalizador");
        Equal(1, h.Tickets.Calls, "replay no repite cookie");
    }

    private static Login2Fa.PendingSecondFactorContext AlterContext(Login2Fa.PendingSecondFactorContext source, int empresaId, int moduloId, string moduleType, long userId, string login)
    {
        return new Login2Fa.PendingSecondFactorContext(empresaId, moduloId, moduleType, userId, login,
            source.MaskedDestination, source.ChallengeId, source.SessionNonce, source.ResendCount,
            source.CreatedAtUtc, source.ExpiresAtUtc);
    }

    private static void ResendAndCancelReplaceAndRevoke()
    {
        var h = new Harness();
        h.Service.Start(h.Request());
        var previous = h.Store.Value.ChallengeId;
        Equal("SECOND_FACTOR_RESEND_COOLDOWN", h.Service.Resend().PublicCode, "cooldown");
        h.Clock.Value = h.Clock.Value.AddSeconds(60);
        var resent = h.Service.Resend();
        Equal("SECOND_FACTOR_RESENT", resent.PublicCode, "reenvío");
        Equal(1, h.Repository.ReplaceCalls, "reemplazo atómico");
        Equal(1, h.Store.Value.ResendCount, "contador");
        Assert(previous != h.Store.Value.ChallengeId, "challenge nuevo");
        h.Clock.Value = h.Clock.Value.AddSeconds(60);
        Equal("SECOND_FACTOR_RESENT", h.Service.Resend().PublicCode, "segundo reenvío");
        h.Clock.Value = h.Clock.Value.AddSeconds(60);
        Equal("SECOND_FACTOR_RESEND_LIMIT", h.Service.Resend().PublicCode, "límite de reenvío");
        Equal(2, h.Store.Value.ResendCount, "máximo dos reenvíos");
        Equal("SECOND_FACTOR_CANCELLED", h.Service.Cancel().PublicCode, "cancelación");
        Equal(1, h.Repository.RevokeCalls, "revocación");
        Equal(null, h.Store.Value, "Session limpia");
        h.Service.Cancel();
        Equal(1, h.Repository.RevokeCalls, "cancelación idempotente");

        var failed = new Harness();
        failed.Service.Start(failed.Request());
        failed.Clock.Value = failed.Clock.Value.AddSeconds(60);
        failed.Email.Success = false;
        Equal("SECOND_FACTOR_DELIVERY_FAILED", failed.Service.Resend().PublicCode, "fallo SMTP de reenvío");
        Equal(null, failed.Store.Value, "Session limpia tras fallo SMTP");
    }

    private static void DiagnosticsAreCorrelatedAndNonBlocking()
    {
        var success = new Harness();
        var request = success.Request();
        Equal(Login2Fa.SecondFactorAuthenticationState.PENDING, success.Service.Start(request).State, "diagnóstico no altera éxito");
        Assert(success.Diagnostics.Entries.Count > 0, "debe emitir etapas");
        Assert(success.Diagnostics.Entries.All(entry => entry.CorrelationId == request.CorrelationId), "una correlación por inicio");
        Assert(success.Diagnostics.Contains(Login2Fa.SecondFactorDiagnosticStage.CHALLENGE_CREATION, Login2Fa.SecondFactorDiagnosticOutcome.SUCCEEDED), "challenge observado");
        Assert(success.Diagnostics.Contains(Login2Fa.SecondFactorDiagnosticStage.EMAIL_DELIVERY, Login2Fa.SecondFactorDiagnosticOutcome.SUCCEEDED), "SMTP observado");
        Assert(success.Diagnostics.Contains(Login2Fa.SecondFactorDiagnosticStage.PENDING_CONTEXT_SAVE, Login2Fa.SecondFactorDiagnosticOutcome.SUCCEEDED), "Session observada");

        success.Service.Verify("123456");
        Assert(success.Diagnostics.Contains(Login2Fa.SecondFactorDiagnosticStage.AUTHENTICATION_VERIFY, Login2Fa.SecondFactorDiagnosticOutcome.SUCCEEDED), "verificación observada");
        Assert(success.Diagnostics.Contains(Login2Fa.SecondFactorDiagnosticStage.TICKET_ISSUANCE, Login2Fa.SecondFactorDiagnosticOutcome.SUCCEEDED), "emisión de cookie observada");

        var rejected = new Harness();
        rejected.Repository.CreateSuccess = false;
        Equal("SECOND_FACTOR_DELIVERY_FAILED", rejected.Service.Start(rejected.Request()).PublicCode, "rechazo funcional intacto");
        Assert(rejected.Diagnostics.Contains(Login2Fa.SecondFactorDiagnosticStage.CHALLENGE_CREATION, Login2Fa.SecondFactorDiagnosticOutcome.REJECTED), "rechazo localizado");

        var nonBlocking = new Harness(true);
        Equal(Login2Fa.SecondFactorAuthenticationState.PENDING, nonBlocking.Service.Start(nonBlocking.Request()).State, "fallo de telemetría no altera login");

        var writer = new System.IO.StringWriter();
        var listener = new System.Diagnostics.TextWriterTraceListener(writer);
        System.Diagnostics.Trace.Listeners.Add(listener);
        try
        {
            var correlation = Guid.NewGuid();
            new Login2Fa.TraceSecondFactorDiagnosticSink().Record(correlation, Login2Fa.SecondFactorDiagnosticStage.CHALLENGE_CREATION, Login2Fa.SecondFactorDiagnosticOutcome.FAILED);
            System.Diagnostics.Trace.Flush();
            var emitted = writer.ToString();
            Assert(emitted.Contains(correlation.ToString("D")), "adapter emite correlación");
            Assert(emitted.Contains("CHALLENGE_CREATION") && emitted.Contains("FAILED"), "adapter emite catálogos");
        }
        finally { System.Diagnostics.Trace.Listeners.Remove(listener); listener.Dispose(); writer.Dispose(); }
    }

    private sealed class Harness
    {
        public readonly Clock Clock = new Clock { Value = new DateTime(2026, 10, 10, 12, 0, 0, DateTimeKind.Utc) };
        public readonly Repository Repository = new Repository();
        public readonly Email Email = new Email();
        public readonly Protector Protector = new Protector();
        public readonly Store Store = new Store();
        public readonly Finalizer Finalizer = new Finalizer();
        public readonly Tickets Tickets = new Tickets();
        public readonly Diagnostics Diagnostics;
        public readonly System.Collections.Generic.List<string> Order = new System.Collections.Generic.List<string>();
        public readonly Login2Fa.SecondFactorAuthenticationService Service;

        public Harness(bool throwingDiagnostics = false)
        {
            Repository.Order = Order;
            Tickets.Order = Order;
            Diagnostics = new Diagnostics { ThrowOnRecord = throwingDiagnostics };
            Service = CreateService(Store);
        }

        public Login2Fa.SecondFactorAuthenticationService CreateService(Store store) { return new Login2Fa.SecondFactorAuthenticationService(Clock, new Otp(), Protector, Repository, Email, new Recipients(), store, Finalizer, Tickets, new Routes(), Diagnostics); }

        public Login2Fa.SecondFactorAuthenticationStartRequest Request()
        {
            var module = new Login2Fa.SecondFactorLoginModule(1, 2, "RAD", "RADICACION DOCUMENTAL", new Login2Fa.SegundoFactorConfiguration(1, 1, 5));
            return new Login2Fa.SecondFactorAuthenticationStartRequest(module, new Login2Fa.SecondFactorPrincipal(3, "LUZ.AGUILERA", "luz@example.test"));
        }
    }

    private sealed class Clock : Login2Fa.ISecondFactorClock { public DateTime Value; public DateTime UtcNow { get { return Value; } } }
    private sealed class Otp : Login2Fa.ISecondFactorOtpGenerator { public string GenerateCode() { return "123456"; } }
    private sealed class Protector : Login2Fa.ISecondFactorCodeProtector
    {
        public bool ThrowOnVerify;
        public string Protect(Login2Fa.SecondFactorProtectionContext context, string code) { return "protected:" + code; }
        public bool Verify(Login2Fa.SecondFactorProtectionContext context, string code, string protectedCode) { if (ThrowOnVerify) throw new InvalidOperationException("synthetic key failure"); return protectedCode == "protected:" + code; }
    }
    private sealed class Email : Login2Fa.ISecondFactorEmailSender
    {
        public int Calls; public bool Success = true;
        public Login2Fa.SecondFactorDeliveryResult Send(Login2Fa.SecondFactorEmailMessage message) { Interlocked.Increment(ref Calls); return new Login2Fa.SecondFactorDeliveryResult { Success = Success, PublicCode = Success ? "SENT" : "FAILED" }; }
    }
    private sealed class Recipients : Login2Fa.ISecondFactorRecipientResolver
    { public Login2Fa.SecondFactorRecipient Resolve(Login2Fa.SegundoFactorIdentity identity) { return new Login2Fa.SecondFactorRecipient("luz@example.test", "l***@example.test"); } }
    private sealed class Store : Login2Fa.IPendingSecondFactorContextStore
    {
        public Login2Fa.PendingSecondFactorContext Value;
        public void Save(Login2Fa.PendingSecondFactorContext context) { Value = context; }
        public Login2Fa.PendingSecondFactorContext GetCurrent() { return Value; }
        public void Clear() { Value = null; }
    }
    private sealed class Finalizer : Login2Fa.ILegacyLoginFinalizer
    {
        public int Calls; public bool Success = true;
        public Login2Fa.LegacyLoginFinalizationResult FinalizeLogin(Login2Fa.LegacyLoginFinalizationContext context) { Interlocked.Increment(ref Calls); return new Login2Fa.LegacyLoginFinalizationResult { Success = Success, PublicCode = Success ? "YES" : "FAILED" }; }
    }
    private sealed class Tickets : Login2Fa.ISecondFactorAuthenticationTicketIssuer
    {
        public int Calls; public bool ThrowOnIssue; public System.Collections.Generic.List<string> Order;
        public void Issue(string normalizedLogin) { if (ThrowOnIssue) throw new InvalidOperationException("synthetic ticket failure"); Interlocked.Increment(ref Calls); lock (Order) Order.Add("ticket"); }
    }
    private sealed class Routes : Login2Fa.ILoginLocalRouteResolver
    { public string Resolve() { return "~/Defaul/WebFormInicioDocuarchiGestion.aspx"; } }

    private sealed class DiagnosticEntry
    {
        public Guid CorrelationId;
        public Login2Fa.SecondFactorDiagnosticStage Stage;
        public Login2Fa.SecondFactorDiagnosticOutcome Outcome;
    }

    private sealed class Diagnostics : Login2Fa.ISecondFactorDiagnosticSink
    {
        public readonly System.Collections.Generic.List<DiagnosticEntry> Entries = new System.Collections.Generic.List<DiagnosticEntry>();
        public bool ThrowOnRecord;
        public void Record(Guid correlationId, Login2Fa.SecondFactorDiagnosticStage stage, Login2Fa.SecondFactorDiagnosticOutcome outcome)
        {
            if (ThrowOnRecord) throw new InvalidOperationException("synthetic diagnostics failure");
            Entries.Add(new DiagnosticEntry { CorrelationId = correlationId, Stage = stage, Outcome = outcome });
        }
        public bool Contains(Login2Fa.SecondFactorDiagnosticStage stage, Login2Fa.SecondFactorDiagnosticOutcome outcome)
        { return Entries.Any(entry => entry.Stage == stage && entry.Outcome == outcome); }
    }

    private sealed class Repository : Login2Fa.ISecondFactorChallengeRepository
    {
        public Login2Fa.SegundoFactorChallenge Challenge; public string ProtectedCode; public string SessionBinding; public bool Consumed; public bool CreateSuccess = true, MarkSentSuccess = true, CompleteSuccess = true;
        public int CreateCalls, MarkSentCalls, GetCalls, BeginCalls, CompleteCalls, ReplaceCalls, RevokeCalls;
        public System.Collections.Generic.List<string> Order;
        public bool Create(Login2Fa.SegundoFactorChallenge challenge, string protectedCode, string sessionBindingHash) { CreateCalls++; Challenge = challenge; ProtectedCode = protectedCode; SessionBinding = sessionBindingHash; Consumed = false; return CreateSuccess; }
        public Login2Fa.SegundoFactorChallenge GetForVerification(Guid id, string binding) { return Challenge; }
        public Login2Fa.SecondFactorChallengeVerificationData GetVerificationData(Guid id, string binding) { GetCalls++; if (Challenge == null || Consumed || Challenge.ChallengeId != id || !String.Equals(SessionBinding, binding, StringComparison.Ordinal)) return null; return Data(); }
        public bool MarkSent(Guid id, DateTime at) { MarkSentCalls++; return MarkSentSuccess && !Consumed && Challenge != null && Challenge.ChallengeId == id; }
        public bool MarkDeliveryFailed(Guid id, DateTime at) { Consumed = true; return true; }
        public Login2Fa.SegundoFactorChallenge RegisterFailedAttempt(Guid id, int expected) { return null; }
        public Login2Fa.SecondFactorStoredChallenge RegisterFailedAttemptData(Guid id, int expected) { lock (this) { if (Consumed || Challenge.Attempts != expected) return null; Challenge = new Login2Fa.SegundoFactorChallenge(Challenge.ChallengeId, Challenge.Identity, Challenge.Purpose, Challenge.State, expected + 1, Challenge.ResendCount, Challenge.CreatedAtUtc, Challenge.ExpiresAtUtc); return Data().Challenge; } }
        public bool TryBeginFinalization(Guid id, int expected) { lock (this) { BeginCalls++; if (Consumed || Challenge.Attempts != expected) return false; Consumed = true; return true; } }
        public bool Complete(Guid id) { CompleteCalls++; lock (Order) Order.Add("complete"); return CompleteSuccess && Consumed; }
        public bool FailFinalization(Guid id) { return Consumed; }
        public bool Revoke(Guid id) { RevokeCalls++; Consumed = true; return true; }
        public bool ReplaceForResend(Guid previous, Login2Fa.SegundoFactorChallenge replacement, string protectedCode, string binding, DateTime requested) { ReplaceCalls++; if (Consumed) return false; Challenge = replacement; ProtectedCode = protectedCode; SessionBinding = binding; Consumed = false; return true; }
        public bool Expire(Guid id, DateTime at) { Consumed = true; return true; }
        private Login2Fa.SecondFactorChallengeVerificationData Data()
        {
            var stored = new Login2Fa.SecondFactorStoredChallenge(Challenge.ChallengeId, Challenge.Identity.ClaveCanonica, Challenge.Purpose, Login2Fa.SegundoFactorChallengeState.SENT, Challenge.Attempts, Challenge.CreatedAtUtc, Challenge.ExpiresAtUtc);
            return new Login2Fa.SecondFactorChallengeVerificationData(stored, ProtectedCode);
        }
    }

    private static void Assert(bool value, string label) { if (!value) throw new InvalidOperationException(label); }
    private static void Equal<T>(T expected, T actual, string label) { if (!Object.Equals(expected, actual)) throw new InvalidOperationException(label + ": esperado=" + expected + ", actual=" + actual); }
}
