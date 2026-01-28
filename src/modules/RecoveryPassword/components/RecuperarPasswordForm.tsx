import { useState } from "react";
import styles from "../../login/Style/login.module.css";
import tokenStyles from "../../../modules/OTP/Style/codeverification.module.css";
import RequiredTooltip from "../../../app/Components/RequiredTooltip";
import { useNavigate } from "react-router";
import type { RecuperarPasswordRequest } from "../Models/RecuperarPasswordRequest";

export default function ForgotPasswordForm({
  onSubmit,
  isLoading = false,
  idModulo,
  idEmpresa,
  loginUsuario,
}: ForgotPasswordFormProps) {
  const [user, setUser] = useState(loginUsuario ?? "");
  const [submitIntentado, setSubmitIntentado] = useState(false);
  const navigate = useNavigate();

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitIntentado(true);

    if (!user || idModulo === 0 || isLoading) return;

    onSubmit({
      user,
      idModule: idModulo,
      IdEmpresa: idEmpresa,
    });
  };

  return (
    <div className={styles.contenedor_}>
      {/* Flecha volver */}
      <div className={tokenStyles["arrow-back"]}>
        <a
          href="#"
          aria-label="Volver"
          onClick={(e) => {
            e.preventDefault();
            navigate("/");
          }}
        >
          <i className="fa-solid fa-angle-left" />
        </a>
      </div>

      {/* Header */}
      <div className={tokenStyles.headerpasw}>
        <div className={tokenStyles.icon}>
          <i className="fa-solid fa-key" />
        </div>

        <h3>Recuperar contraseña</h3>
      </div>

      {/* Bloque de ancho controlado */}
      <div className={tokenStyles.formWidth}>
        <p className={tokenStyles.messagepasw}>
          Te enviaremos un código de verificación al correo asociado a tu usuario.
        </p>

        <form onSubmit={handleSubmit} noValidate>
          {/* Usuario */}
          <div className={styles["input-contenedor"]}>
            <i className="fa-solid fa-user" />

            <input
              type="text"
              placeholder=" "
              value={user}
              autoComplete="username"
              onChange={(e) => setUser(e.target.value)}
              aria-invalid={submitIntentado && !user}
              disabled={isLoading}
            />

            <label>Usuario</label>

            <RequiredTooltip
              visible={submitIntentado && !user}
              message="Debe informar el usuario"
            />
          </div>

          {/* Botón */}
          <div className={styles.aaa}>
            <button type="submit" disabled={isLoading}>
              {isLoading ? "Procesando..." : "Continuar"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

/* =======================
   Tipos
======================= */

type ForgotPasswordFormProps = {
  onSubmit: (data: RecuperarPasswordRequest) => void;
  isLoading?: boolean;
  idModulo: number;
  idEmpresa: number;
  loginUsuario?: string;
  eMail?: string;
};
