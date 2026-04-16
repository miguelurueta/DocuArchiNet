import { useEffect, useId, useRef, useState } from "react";
import { AppButton } from "../../../../../app/Components/UI/AppButton";
import { AppInput } from "../../../../../app/Components/UI/AppInput";
import { AppInputSelect } from "../../../../../app/Components/UI/AppInputSelect";
import { AppInputTags } from "../../../../../app/Components/UI/AppInputTags";
import { AppModal } from "../../../../../app/Components/UI/AppModal";
import styles from "./GestionDocumentoModal.module.css";

type GestionDocumentoModalProps = {
  open: boolean;
  onClose: () => void;
};

const tipoDocumentoOptions = [
  { label: "Solicitud de aprobacion", value: "solicitud-aprobacion" },
  { label: "Memorando interno", value: "memorando-interno" },
  { label: "Respuesta tecnica", value: "respuesta-tecnica" },
];

const tagsOptions = [
  { label: "Juridica", value: "Juridica" },
  { label: "Urgente", value: "Urgente" },
  { label: "Aprobacion", value: "Aprobacion" },
];

export function GestionDocumentoModal({ open, onClose }: GestionDocumentoModalProps) {
  const titleId = useId();
  const initialFocusRef = useRef<HTMLButtonElement | null>(null);
  const [tipoDocumento, setTipoDocumento] = useState<string | number | undefined>();
  const [requiereFirma, setRequiereFirma] = useState(false);
  const [etiquetas, setEtiquetas] = useState<string[]>([]);

  useEffect(() => {
    if (open) {
      setTimeout(() => {
        initialFocusRef.current?.focus();
      }, 0);
    }
  }, [open]);

  return (
    <AppModal
      open={open}
      onClose={onClose}
      title={<span id={titleId}>Gestionar Documento</span>}
      hideFooter
      width={760}
      destroyOnHidden
    >
      <div className={styles.container}>
        <div className={styles.infoBox}>
          <h3 className={styles.infoTitle}>Solicitud de aprobacion documental</h3>
          <p className={styles.infoCopy}>
            Configure los metadatos principales del documento y defina el contexto de
            la solicitud antes de continuar con el flujo operativo.
          </p>
          <div className={styles.infoMeta}>
            <span>Origen: Gestion respuesta</span>
            <span>Tipo: Documento interno</span>
            <span>Estado inicial: Borrador</span>
          </div>
        </div>

        <div className={styles.formGrid}>
          <AppInputSelect
            label="Tipo de documento"
            aria-labelledby={titleId}
            placeholder="Seleccione el tipo de documento"
            options={tipoDocumentoOptions}
            value={tipoDocumento}
            onChange={(value) => setTipoDocumento(value as string | number | undefined)}
            size="md"
          />

          <AppInput
            type="checkbox"
            label="Requiere firma del responsable"
            checked={requiereFirma}
            onChange={(event) => setRequiereFirma(event.target.checked)}
          />

          <AppInputTags
            label="Etiquetas de gestion"
            aria-labelledby={titleId}
            placeholder="Agregar etiquetas"
            options={tagsOptions}
            value={etiquetas}
            onAddTag={(tag) =>
              setEtiquetas((current) => (current.includes(tag) ? current : [...current, tag]))
            }
            onRemoveTag={(tag) =>
              setEtiquetas((current) => current.filter((currentTag) => currentTag !== tag))
            }
            onRemoveAll={() => setEtiquetas([])}
          />
        </div>

        <div className={styles.actions}>
          <AppButton ref={initialFocusRef} variant="secondary" onClick={onClose}>
            Cancelar
          </AppButton>
          <AppButton onClick={onClose}>Guardar</AppButton>
        </div>
      </div>
    </AppModal>
  );
}
