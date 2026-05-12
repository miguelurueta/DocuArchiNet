import { memo, useEffect, useRef, useState } from "react";
import { EyeInvisibleOutlined, EyeOutlined } from "@ant-design/icons";

import styles from "./AppPdfPasswordPrompt.module.css";

export interface AppPdfPasswordPromptProps {
  isInvalidPassword?: boolean;
  isLoading?: boolean;
  onSubmit(password: string): void;
}

export const AppPdfPasswordPrompt = memo(function AppPdfPasswordPrompt({
  isInvalidPassword = false,
  isLoading = false,
  onSubmit,
}: AppPdfPasswordPromptProps) {
  const [password, setPassword] = useState("");
  const [isVisible, setIsVisible] = useState(false);
  const inputRef = useRef<HTMLInputElement | null>(null);

  useEffect(() => {
    inputRef.current?.focus();
  }, []);

  return (
    <div className={styles.shell} role="dialog" aria-label="Documento protegido">
      <div className={styles.panel}>
        <h3 className={styles.title}>Documento protegido</h3>
        <p className={styles.hint}>
          Ingresa la contraseña para abrir el PDF.
        </p>

        <div className={styles.row}>
          <div className={styles.inputShell}>
            <input
              ref={inputRef}
              className={styles.input}
              type={isVisible ? "text" : "password"}
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              aria-label="Contraseña del documento"
              // Evitar autofill del navegador/gestor de contraseñas en un prompt interno de visor.
              // Nota: algunos password managers ignoran `off`, por eso usamos señales adicionales.
              autoComplete="new-password"
              name="embedpdf-document-password"
              data-lpignore="true"
              data-1p-ignore="true"
              data-form-type="other"
              disabled={isLoading}
            />
            <button
              type="button"
              className={styles.eye}
              onClick={() => setIsVisible((v) => !v)}
              aria-label={isVisible ? "Ocultar contraseña" : "Mostrar contraseña"}
              title={isVisible ? "Ocultar contraseña" : "Mostrar contraseña"}
              disabled={isLoading}
            >
              {isVisible ? <EyeInvisibleOutlined /> : <EyeOutlined />}
            </button>
          </div>
          <button
            type="button"
            className={styles.button}
            onClick={() => onSubmit(password)}
            aria-label="Continuar"
            title="Continuar"
            disabled={isLoading || !password.trim()}
          >
            {isLoading ? "Validando…" : "Continuar"}
          </button>
        </div>

        {isInvalidPassword ? (
          <div className={styles.error} role="alert" aria-label="Contraseña inválida">
            Contraseña inválida. Intenta nuevamente.
          </div>
        ) : null}
      </div>
    </div>
  );
});
