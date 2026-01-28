import { useLocation } from "react-router";
import ForgotPasswordForm from "../components/RecuperarPasswordForm";
import { useRecuperarPasswordMutation } from "../hooks/useRecuperarPasswordMutation";
import type { RecuperarPasswordRequest } from "../Models/RecuperarPasswordRequest";

export default function ForgotPasswordPage() {
  const { state } = useLocation();
  const mutation = useRecuperarPasswordMutation();
  const handleSubmit = (data: RecuperarPasswordRequest) => {
    mutation.mutate(data);
  };

  return (
    <ForgotPasswordForm
      onSubmit={handleSubmit}
      isLoading={false}
      idModulo={state.idModulo}
      idEmpresa={state.idEmpresa}
      loginUsuario={state.loginUsuario}
    />
  );
}
