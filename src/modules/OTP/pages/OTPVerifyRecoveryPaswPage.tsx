import { useNavigate, useLocation } from "react-router";
import { useOTPRecoveryVeryOptPaswMutation } from "../hooks/useOTPRecoveryVeryOptPaswMutation";
import FormularioToken from "../components/FormularioToken";

export default function OTPVerifyRecoveryPaswPage() {
    const navigate = useNavigate();
    const { state } = useLocation();
    const mutation = useOTPRecoveryVeryOptPaswMutation();
    const handleSubmit = (Code: string) => {
    mutation.mutate({
      ChallengeId: state.payload[0].challengeId,
      Code:Code
    });
  };
  const handleBack = () => {
  navigate("/");
};
const handleExpired = () => {
  navigate("/", { state: { reason: "OTP_EXPIRED" } });
};
const tiempoExpiraMin = Number(state.payload[0].tiempoExpira) || 0;
return (
<>
<FormularioToken
    email={state.payload[0].destinoEnmascarado}
    tiempoExpira={tiempoExpiraMin}
    expired={false}
    onBackNavigate={handleBack}
    onExpiredNavigate={handleExpired}
    onSubmit={handleSubmit}
  />
</>
);
}