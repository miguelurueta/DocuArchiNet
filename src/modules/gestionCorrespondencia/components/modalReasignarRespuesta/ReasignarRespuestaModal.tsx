import { SwapOutlined } from "@ant-design/icons";
import { useEffect, useId, useMemo, useRef } from "react";
import { AppButton } from "../../../../app/Components/UI/AppButton";
import { AppInputTags } from "../../../../app/Components/UI/AppInputTags";
import { AppModal } from "../../../../app/Components/UI/AppModal";
import styles from "./ReasignarRespuestaModal.module.css";

export type ReasignarRespuestaModalProps = {
  open: boolean;
  onClose: () => void;
  radicado: string;
  nota: string;
  users: string[];
  onAddUser: (value: string) => void;
  onRemoveUser: (value: string) => void;
  onRemoveAllUsers: () => void;
  onSubmit: () => void;
};

const RESPONSABLE_SUGGESTIONS = [
  { label: "Angelica Torres (angelica.torres@contasoft.com)", value: "angelica.torres@contasoft.com" },
  { label: "Carlos Vega (carlos.vega@contasoft.com)", value: "carlos.vega@contasoft.com" },
  { label: "Laura Mendoza (laura.mendoza@contasoft.com)", value: "laura.mendoza@contasoft.com" },
  { label: "Sofia Rojas (sofia.rojas@contasoft.com)", value: "sofia.rojas@contasoft.com" },
  { label: "Juan Pardo (juan.pardo@contasoft.com)", value: "juan.pardo@contasoft.com" },
];

export function ReasignarRespuestaModal({
  open,
  onClose,
  radicado,
  nota,
  users,
  onAddUser,
  onRemoveUser,
  onRemoveAllUsers,
  onSubmit,
}: ReasignarRespuestaModalProps) {
  const titleId = useId();
  const primaryButtonRef = useRef<HTMLButtonElement | null>(null);
  const responsableOptions = useMemo(
    () => [
      ...RESPONSABLE_SUGGESTIONS,
      ...users
        .filter((value) => !RESPONSABLE_SUGGESTIONS.some((item) => item.value === value))
        .map((value) => ({ label: value, value })),
    ],
    [users],
  );

  useEffect(() => {
    if (!open) return;
    setTimeout(() => primaryButtonRef.current?.focus(), 0);
  }, [open]);

  return (
    <AppModal
      open={open}
      onClose={onClose}
      title={<span id={titleId}>Reasignar Respuesta</span>}
      centered
      width="min(720px, 92vw)"
      className={styles.modal}
      wrapClassName={styles.wrap}
      hideFooter
      destroyOnHidden
    >
      <section className={styles.container} aria-labelledby={titleId}>
        <header className={styles.header}>
          <div className={styles.titleGroup}>
            <span className={styles.iconWrap} aria-hidden="true">
              <SwapOutlined />
            </span>
            <h3 className={styles.title}>Reasignar Respuesta</h3>
          </div>
          <span className={styles.meta}>RAD. {radicado}</span>
        </header>

        <AppInputTags
          label="Responsable"
          value={users}
          options={responsableOptions}
          helperText="Escribe nombre o correo, selecciona sugerencia y presiona Enter para agregar."
          placeholder="Seleccionar responsable"
          onAddTag={onAddUser}
          onRemoveTag={onRemoveUser}
          onRemoveAll={onRemoveAllUsers}
          aria-labelledby={titleId}
        />

        <section className={styles.noteSection} aria-label="Nota">
          <div className={styles.noteHeader}>
            <span className={styles.noteLabel}>Nota</span>
            <span className={styles.noteDivider} aria-hidden="true" />
          </div>
          <div className={styles.noteBox}>
            <p className={styles.noteLead}>Contexto del tramite a reasignar</p>
            <p className={styles.noteText}>{nota}</p>
          </div>
        </section>

        <footer className={styles.actions} aria-label="Acciones">
          <AppButton variant="secondary" onClick={onClose}>
            Cancelar
          </AppButton>
          <AppButton ref={primaryButtonRef} onClick={onSubmit}>
            Enviar
          </AppButton>
        </footer>
      </section>
    </AppModal>
  );
}
