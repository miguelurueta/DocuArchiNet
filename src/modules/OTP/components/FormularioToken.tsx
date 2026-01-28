import { useEffect, useRef, useState } from "react";
import Modal from "@mui/material/Modal";
import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import Typography from "@mui/material/Typography";
import styles from "../Style/codeverification.module.css";

type FormularioTokenProps = {
  email: string;
  onSubmit: (token: string) => void;

  /** ⏱️ tiempo en MINUTOS */
  tiempoExpira: number;

  /** expiración forzada desde backend */
  expired: boolean;

  /** navegación flecha volver */
  onBackNavigate?: () => void;

  /** navegación al expirar */
  onExpiredNavigate?: () => void;

  expiredModalTitle?: string;
  expiredModalMessage?: string;
  expiredModalButtonText?: string;
};

export default function FormularioToken({
  email,
  onSubmit,
  tiempoExpira,
  expired,
  onBackNavigate,
  onExpiredNavigate,
  expiredModalTitle,
  expiredModalMessage,
  expiredModalButtonText,
}: FormularioTokenProps) {
  // =====================================================
  // FORMULARIO (SIN CAMBIOS)
  // =====================================================
  const [code, setCode] = useState<string[]>(Array(6).fill(""));
  const inputsRef = useRef<(HTMLInputElement | null)[]>([]);
  const [localError, setLocalError] = useState("");

  // =====================================================
  // ⏱️ CONTADOR SIMPLE (FUNCIONAL)
  // =====================================================
  const initialSeconds = Math.max(0, Number(tiempoExpira || 0) * 60);

  const [remainingSeconds, setRemainingSeconds] =
    useState<number>(initialSeconds);

  const [expiredLocal, setExpiredLocal] = useState(false);

  // =====================================================
  // ⏱️ Countdown (NO SE TOCA)
  // =====================================================
  useEffect(() => {
    if (expired) {
      setRemainingSeconds(0);
      setExpiredLocal(true);
      return;
    }

    setRemainingSeconds(initialSeconds);
    setExpiredLocal(false);

    if (initialSeconds <= 0) {
      setExpiredLocal(true);
      return;
    }

    const interval = setInterval(() => {
      setRemainingSeconds((prev) => {
        if (prev <= 1) {
          clearInterval(interval);
          setExpiredLocal(true);
          return 0;
        }
        return prev - 1;
      });
    }, 1000);

    return () => clearInterval(interval);
  }, [initialSeconds, expired]);

  // =====================================================
  // 🛑 MODAL EXPIRACIÓN (NUEVO, AISLADO)
  // =====================================================
  const [openExpiredModal, setOpenExpiredModal] = useState(false);

  useEffect(() => {
    if (expiredLocal) {
      setOpenExpiredModal(true);
    }
  }, [expiredLocal]);

  const isExpiredNow = expired || expiredLocal;

  // =====================================================
  // Helpers
  // =====================================================
  function formatTime(seconds: number): string {
    const min = Math.floor(seconds / 60);
    const sec = seconds % 60;
    return `${min}:${sec.toString().padStart(2, "0")}`;
  }

  // =====================================================
  // MANEJO INPUTS (IGUAL)
  // =====================================================
  const handleChange = (value: string, index: number) => {
    if (!/^\d?$/.test(value)) return;

    const next = [...code];
    next[index] = value;
    setCode(next);

    if (localError) setLocalError("");

    if (value && index < 5) {
      inputsRef.current[index + 1]?.focus();
    }
  };

  const handleKeyDown = (
    e: React.KeyboardEvent<HTMLInputElement>,
    index: number
  ) => {
    if (e.key === "Backspace" && !code[index] && index > 0) {
      inputsRef.current[index - 1]?.focus();
    }
  };

  const handlePaste = (e: React.ClipboardEvent<HTMLInputElement>) => {
    e.preventDefault();
    const pasted = e.clipboardData.getData("text").trim().slice(0, 6);
    if (!/^\d+$/.test(pasted)) return;

    const arr = pasted.split("");
    setCode([...arr, ...Array(6 - arr.length).fill("")]);
    inputsRef.current[Math.min(arr.length - 1, 5)]?.focus();
  };

  // =====================================================
  // SUBMIT (IGUAL)
  // =====================================================
  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    if (isExpiredNow) return;

    const finalCode = code.join("");
    if (finalCode.length < 6) {
      setLocalError("Completa el código de 6 dígitos.");
      return;
    }

    setLocalError("");
    onSubmit(finalCode);
  };

  // =====================================================
  // RENDER
  // =====================================================
  return (
    <>
      <div className={styles.container}>
        {/* Flecha volver */}
        <div className={styles["arrow-back"]}>
          <a
            href="#"
            onClick={(e) => {
              e.preventDefault();
              onBackNavigate?.();
            }}
            aria-label="Volver"
          >
            <i className="fa-solid fa-angle-left"></i>
          </a>
        </div>

        <div className={styles.header}>
          <div className={styles.icon}>
            <i className="fa-solid fa-envelope"></i>
          </div>

          <h3>¡VERIFICA TU CORREO!</h3>

          <p style={{ textAlign: "left" }}>
            Te enviamos un correo a{" "}
            <strong>{email}</strong>. Recuerda que este código vence en{" "}
            <span
              style={{
                display: "inline-block",
                minWidth: "3.5ch",
                textAlign: "right",
                fontVariantNumeric: "tabular-nums",
                fontFamily: "monospace",
              }}
            >
              {formatTime(remainingSeconds)}
            </span>
            .
          </p>

          <p className={styles.eac}>
            <b>Escríbelo a continuación:</b>
          </p>
        </div>

        <form onSubmit={handleSubmit}>
          <div className={styles.pin}>
            {code.map((digit, index) => (
              <input
                key={index}
                    ref={(el) => {
                        inputsRef.current[index] = el
                    }}
                    type="text"
                inputMode="numeric"
                autoComplete={index === 0 ? "one-time-code" : "off"}
                maxLength={1}
                disabled={isExpiredNow}
                className={styles["code-input"]}
                value={digit}
                onChange={(e) => handleChange(e.target.value, index)}
                onKeyDown={(e) => handleKeyDown(e, index)}
                onPaste={handlePaste}
                required
              />
            ))}
          </div>

          {localError && (
            <p style={{ marginTop: 10, color: "red", textAlign: "center" }}>
              {localError}
            </p>
          )}

          <div className={styles["btn-wrap"]}>
            <button type="submit" disabled={isExpiredNow}>
              Validar Código
            </button>
          </div>
        </form>
      </div>

      {/* ==========================
          🛑 MODAL MUI – EXPIRACIÓN
         ========================== */}
      <Modal open={openExpiredModal}>
        <Box
          sx={{
            position: "absolute",
            top: "50%",
            left: "50%",
            transform: "translate(-50%, -50%)",
            bgcolor: "background.paper",
            borderRadius: 2,
            boxShadow: 24,
            p: 3,
            width: 380,
            maxWidth: "90%",
          }}
        >
          <Typography variant="h6" gutterBottom>
            {expiredModalTitle ?? "Código vencido"}
          </Typography>

          <Typography sx={{ mb: 2 }}>
            {expiredModalMessage ??
              "El código de verificación ha expirado por seguridad. Debes volver al formulario anterior para solicitar uno nuevo."}
          </Typography>

          <Box sx={{ display: "flex", justifyContent: "flex-end" }}>
            <Button
              variant="contained"
              onClick={() => {
                setOpenExpiredModal(false);
                onExpiredNavigate?.();
              }}
            >
              {expiredModalButtonText ?? "Volver"}
            </Button>
          </Box>
        </Box>
      </Modal>
    </>
  );
}
