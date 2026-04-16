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

const prioridadOptions = [
  { label: "Alta", value: "alta" },
  { label: "Media", value: "media" },
  { label: "Baja", value: "baja" },
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
  const [prioridadDocumento, setPrioridadDocumento] = useState<
    string | number | undefined
  >();
  const [requiereFirma, setRequiereFirma] = useState(false);
  const [requiereAnexos, setRequiereAnexos] = useState(true);
  const [notificarResponsable, setNotificarResponsable] = useState(false);
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
      title={<span id={titleId}>Confirmar envio de respuesta</span>}
      hideFooter
      width={760}
      destroyOnHidden
    >
      <div className={styles.container}>
        <div className={styles.checksGroup}>
          <AppInput
            type="checkbox"
            label="Solicita al centro de envio de correspondencia el envio de la respuesta"
            checked={requiereFirma}
            onChange={(event) => setRequiereFirma(event.target.checked)}
          />

          <AppInput
            type="checkbox"
            label="Confirma respuesta al correo electronico del peticionario"
            checked={requiereAnexos}
            onChange={(event) => setRequiereAnexos(event.target.checked)}
          />

          <AppInput
            type="checkbox"
            label="Certificar digitalmente el documento de respuesta"
            checked={notificarResponsable}
            onChange={(event) => setNotificarResponsable(event.target.checked)}
          />
        </div>

        <div className={styles.contentGrid}>
          <div className={styles.selectsColumn}>
            <AppInputSelect
              label="Firma de la respuesta"
              aria-labelledby={titleId}
              placeholder="Seleccione la firma de la respuesta"
              options={tipoDocumentoOptions}
              value={tipoDocumento}
              onChange={(value) => setTipoDocumento(value as string | number | undefined)}
              size="md"
            />

            <AppInputSelect
              label="Tipo de respuesta"
              aria-labelledby={titleId}
              placeholder="Seleccione el tipo de respuesta"
              options={prioridadOptions}
              value={prioridadDocumento}
              onChange={(value) =>
                setPrioridadDocumento(value as string | number | undefined)
              }
              size="md"
            />
          </div>

          <div className={styles.infoBox}>
            <div className={styles.infoMeta}>
              <div className={styles.infoItem}>
                <span className={styles.infoLabel}>Radicado</span>
                <strong className={styles.infoValue}>2500895748</strong>
              </div>
              <div className={styles.infoItem}>
                <span className={styles.infoLabel}>Fecha de vencimiento</span>
                <strong className={styles.infoValue}>20/05/2026</strong>
              </div>
              <div className={styles.infoItem}>
                <span className={styles.infoLabel}>Cantidad documentos adjuntos</span>
                <strong className={styles.infoValue}>2</strong>
              </div>
            </div>
          </div>
        </div>

        <AppInputTags
          label="Direccion de correos"
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

        <div className={styles.actions}>
          <AppButton ref={initialFocusRef} variant="secondary" onClick={onClose}>
            Cancelar
          </AppButton>
          <AppButton onClick={onClose}>Confirmar envio</AppButton>
        </div>
      </div>
    </AppModal>
  );
}
