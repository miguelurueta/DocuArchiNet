import { useState } from "react";
import styles from "../../login/Style/login.module.css";
import tokenStyles from "../../../modules/OTP/Style/codeverification.module.css";
import RequiredTooltip from "../../../app/Components/RequiredTooltip";
import { useNavigate } from "react-router";

export default function CambiarPasswordForm({
  onSubmit,
}: ResetPasswordFormProps) {
  const [newPassword, setNewPassword] = useState("");
  const [confirm, setConfirm] = useState("");
  const [showPassword, setShowPassword] = useState(false);

  const [submitIntentado, setSubmitIntentado] = useState(false);
  const [localError, setLocalError] = useState("");

  const navigate = useNavigate();

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitIntentado(true);

    if (!newPassword || !confirm) return;

    if (newPassword !== confirm) {
      setLocalError("Las contraseñas no coinciden");
      return;
    }

    setLocalError("");
    onSubmit(newPassword, confirm);
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
        <h3>Nueva contraseña</h3>
      </div>

      {/* Bloque ancho controlado */}
      <div className={tokenStyles.formWidth}>
        <form onSubmit={handleSubmit} noValidate>
          {/* Nueva contraseña */}
          <div className={styles["input-contenedor"]}>
            <i className="fa-solid fa-lock" />

            <input
              type={showPassword ? "text" : "password"}
              placeholder=" "
              value={newPassword}
              onChange={(e) => setNewPassword(e.target.value)}
              aria-invalid={submitIntentado && !newPassword}
            />

            <label>Nueva contraseña</label>

            <RequiredTooltip
              visible={submitIntentado && !newPassword}
              message="Debe informar la contraseña"
            />
          </div>

          {/* Confirmar contraseña */}
          <div className={styles["input-contenedor"]}>
            <i className="fa-solid fa-lock" />

            <input
              type={showPassword ? "text" : "password"}
              placeholder=" "
              value={confirm}
              onChange={(e) => setConfirm(e.target.value)}
              aria-invalid={submitIntentado && !confirm}
            />

            <label>Confirmar contraseña</label>

            <RequiredTooltip
              visible={submitIntentado && !confirm}
              message="Debe confirmar la contraseña"
            />
          </div>

          {/* Toggle ver contraseña (MISMO patrón que Login) */}
          <div className={styles["switch-contenedor"]}>
            <label className={styles.switch}>
              <input
                type="checkbox"
                checked={showPassword}
                onChange={() => setShowPassword((v) => !v)}
              />
              <span className={styles.slider}></span>
            </label>
          </div>

          {/* Error local (solo mismatch) */}
          {localError && (
            <p
              style={{
                color: "#d32f2f",
                textAlign: "center",
                marginTop: "0.5rem",
              }}
            >
              {localError}
            </p>
          )}

          {/* Botón */}
          <div className={styles.aaa}>
            <button type="submit">Actualizar contraseña</button>
          </div>
        </form>
      </div>
    </div>
  );
}

/* =======================
   Tipos
======================= */

type ResetPasswordFormProps = {
  onSubmit: (newPassword: string, confirm: string) => void;
};
