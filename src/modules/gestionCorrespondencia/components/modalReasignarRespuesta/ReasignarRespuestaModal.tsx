import { SwapOutlined } from "@ant-design/icons";
import { useEffect, useId, useMemo, useRef, useState } from "react";
import { AppButton } from "../../../../app/Components/UI/AppButton";
import { AppInputTags } from "../../../../app/Components/UI/AppInputTags";
import { AppModal } from "../../../../app/Components/UI/AppModal";
import { TramiteReasignadoModal } from "../modalTramiteReasignado/TramiteReasignadoModal";
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
  const [attemptedSubmit, setAttemptedSubmit] = useState(false);
  const [isConfirmationOpen, setIsConfirmationOpen] = useState(false);
  const [confirmationUser, setConfirmationUser] = useState("");
  const responsableOptions = useMemo(
    () => [
      ...RESPONSABLE_SUGGESTIONS,
      ...users
        .filter((value) => !RESPONSABLE_SUGGESTIONS.some((item) => item.value === value))
        .map((value) => ({ label: value, value })),
    ],
    [users],
  );
  const selectedUsers = users.slice(0, 1);

  useEffect(() => {
    if (!open) return;
    setTimeout(() => primaryButtonRef.current?.focus(), 0);
  }, [open]);

  useEffect(() => {
    if (open) {
      setAttemptedSubmit(false);
    }
  }, [open]);

  const missingResponsables = users.length === 0;
  const resolveAssignedUserLabel = (value: string) => {
    const match = RESPONSABLE_SUGGESTIONS.find((item) => item.value === value);
    if (!match) return value;
    const [name] = match.label.split(" (");
    return name?.trim() || value;
  };

  return (
    <>
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
            value={selectedUsers}
            className={selectedUsers.length > 0 ? styles.lockedInputTags : undefined}
            options={responsableOptions}
            openOnFocus={selectedUsers.length === 0}
            closeOnSelect
            inputReadOnly={selectedUsers.length > 0}
            error={attemptedSubmit && missingResponsables}
            state={attemptedSubmit && missingResponsables ? "error" : "default"}
            helperText={
              attemptedSubmit && missingResponsables
                ? "Debe seleccionar al menos un responsable."
                : "Seleccione un usuario o responsable."
            }
            placeholder="Seleccionar usuario o responsable"
            onAddTag={(value) => {
              const current = selectedUsers[0];
              if (current === value) return;
              if (current) {
                onRemoveAllUsers();
              }
              onAddUser(value);
            }}
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
            <AppButton
              ref={primaryButtonRef}
              onClick={() => {
                setAttemptedSubmit(true);
                if (missingResponsables) return;
                const firstAssigned = users[0];
                setConfirmationUser(
                  firstAssigned ? resolveAssignedUserLabel(firstAssigned) : "Sin asignar",
                );
                setIsConfirmationOpen(true);
                onSubmit();
              }}
            >
              Enviar
            </AppButton>
          </footer>
        </section>
      </AppModal>

      <TramiteReasignadoModal
        open={isConfirmationOpen}
        usuarioAsignado={confirmationUser}
        radicado={radicado}
        onClose={() => setIsConfirmationOpen(false)}
      />
    </>
  );
}
