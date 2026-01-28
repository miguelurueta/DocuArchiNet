import { useLocation, useNavigate } from "react-router";
import FormularioToken from "../components/FormularioToken";
import { useOTPVerifyMutation } from "../hooks/useOTPVerifyMutation";

export default function OTPVerifyPage() {
  const navigate = useNavigate();
  const { state } = useLocation() 
  const mutation = useOTPVerifyMutation();
  const handleSubmit = (Code: string) => {
    mutation.mutate({
      ChallengeId: state.payload.data.ChallengeId,
      Code:Code,
    });
  };
const handleBack = () => {
  navigate("/");
};

const handleExpired = () => {
  navigate("/", { state: { reason: "OTP_EXPIRED" } });
};
const tiempoExpiraMin = Number(state.payload.data.TiempoExpira) || 0;
return (
  <FormularioToken
    email={state.payload.data.DestinoEnmascarado}
    tiempoExpira={tiempoExpiraMin}
    expired={false}
    onBackNavigate={handleBack}
    onExpiredNavigate={handleExpired}
    onSubmit={handleSubmit}
  />
);
}
