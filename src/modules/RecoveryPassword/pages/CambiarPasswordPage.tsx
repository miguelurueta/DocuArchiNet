import { useLocation } from "react-router";
import { useCambiarPasswordMutation } from "../hooks/useCambiarPasswordMutation";
import CambiarPasswordForm from "../components/CambiarPasswordForm";

export default function CambiarPasswordPage() {
  const { state } = useLocation();
  const mutation = useCambiarPasswordMutation();
  const handleSubmit = (password: string) => {
    mutation.mutate({
      token: state.token,
      idModule: state.idModule,
       userId:state.userId,
       newPassword:password,
       confirmNewPassword:password
    });
  };
  return <CambiarPasswordForm onSubmit={handleSubmit} />;
}
