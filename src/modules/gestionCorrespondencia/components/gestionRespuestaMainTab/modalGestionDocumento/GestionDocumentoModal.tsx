import { useEffect, useId, useState } from "react";
import { AppCheckbox } from "../../../../../app/Components/UI/AppCheckbox";
import { AppInputSelect } from "../../../../../app/Components/UI/AppInputSelect";
import { AppInputTags } from "../../../../../app/Components/UI/AppInputTags";
import { AppModal } from "../../../../../app/Components/UI/AppModal";
import RequiredTooltip from "../../../../../app/Components/RequiredTooltip";
import { ConfirmacionEnvioModal } from "./ConfirmacionEnvioModal";
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

const MOBILE_QUERY = "(max-width: 768px)";

const useMediaQuery = (query: string) => {
  const getMatches = () =>
    typeof window !== "undefined" ? window.matchMedia(query).matches : false;
  const [matches, setMatches] = useState(getMatches);

  useEffect(() => {
    const mediaQueryList = window.matchMedia(query);
    const update = (event: MediaQueryListEvent) => setMatches(event.matches);
    setMatches(mediaQueryList.matches);
    mediaQueryList.addEventListener("change", update);
    return () => {
      mediaQueryList.removeEventListener("change", update);
    };
  }, [query]);

  return matches;
};

export function GestionDocumentoModal({ open, onClose }: GestionDocumentoModalProps) {
  const titleId = useId();
  const isMobile = useMediaQuery(MOBILE_QUERY);
  const [tipoDocumento, setTipoDocumento] = useState<string | undefined>();
  const [prioridadDocumento, setPrioridadDocumento] = useState<string | undefined>();
  const [solicitaCentroEnvio, setSolicitaCentroEnvio] = useState(false);
  const [confirmaCorreoPeticionario, setConfirmaCorreoPeticionario] = useState(true);
  const [certificaDigitalmente, setCertificaDigitalmente] = useState(false);
  const [etiquetas, setEtiquetas] = useState<string[]>([]);
  const [attemptedSubmit, setAttemptedSubmit] = useState(false);
  const [isConfirmacionOpen, setIsConfirmacionOpen] = useState(false);
  const [correoConfirmado, setCorreoConfirmado] = useState("");

  useEffect(() => {
    if (open) {
      setAttemptedSubmit(false);
    }
  }, [open]);

  const missingFirma = !tipoDocumento;
  const missingTipo = !prioridadDocumento;
  const missingCorreos = etiquetas.length === 0;
  const isFormValid = !missingFirma && !missingTipo && !missingCorreos;
  const radicado = "2500895748";
  const destinatario = "Contasoft Company";

  const mobileModalHeight =
    "calc(100dvh - 2rem - env(safe-area-inset-top) - env(safe-area-inset-bottom))";
  const desktopModalMaxHeight = "min(84vh, 760px)";

  return (
    <>
      <AppModal
        open={open}
        onClose={onClose}
        title={<span id={titleId}>Confirmar envio de respuesta</span>}
        centered
        width={isMobile ? "min(760px, 92vw)" : "min(920px, 92vw)"}
        className={styles.modal}
        wrapClassName={styles.modalWrap}
        styles={{
          container: {
            display: "flex",
            flexDirection: "column",
            height: isMobile ? mobileModalHeight : "auto",
            maxHeight: isMobile ? mobileModalHeight : desktopModalMaxHeight,
            overflow: "hidden",
          },
          header: { flex: "0 0 auto" },
          footer: { flex: "0 0 auto" },
          body: {
            flex: "1 1 auto",
            minHeight: 0,
            overflow: "auto",
            WebkitOverflowScrolling: "touch",
            overscrollBehavior: "contain",
          },
        }}
        secondaryAction={{ label: "Cancelar", onClick: onClose }}
        primaryAction={{
          label: "Confirmar envio",
          onClick: () => {
            setAttemptedSubmit(true);
            if (!isFormValid) return;
            setCorreoConfirmado(etiquetas.join(", "));
            setIsConfirmacionOpen(true);
            onClose();
          },
        }}
        destroyOnHidden
      >
        <div className={styles.container}>
        <div className={styles.checksGroup}>
          <AppCheckbox
            checked={solicitaCentroEnvio}
            label="Solicita al centro de envio de correspondencia el envio de la respuesta"
            onChange={(checked) => setSolicitaCentroEnvio(checked)}
            size="md"
          />

          <AppCheckbox
            checked={confirmaCorreoPeticionario}
            label="Confirma respuesta al correo electronico del peticionario"
            onChange={(checked) => setConfirmaCorreoPeticionario(checked)}
            size="md"
          />

          <AppCheckbox
            checked={certificaDigitalmente}
            label="Certificar digitalmente el documento de respuesta"
            onChange={(checked) => setCertificaDigitalmente(checked)}
            size="md"
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
              onChange={(value) => setTipoDocumento(value as string | undefined)}
              size="md"
              error={attemptedSubmit && missingFirma}
              helperText={
                <RequiredTooltip
                  visible={attemptedSubmit && missingFirma}
                  inline
                  message="Debe seleccionar la firma."
                />
              }
            />

            <AppInputSelect
              label="Tipo de respuesta"
              aria-labelledby={titleId}
              placeholder="Seleccione el tipo de respuesta"
              options={prioridadOptions}
              value={prioridadDocumento}
              onChange={(value) => setPrioridadDocumento(value as string | undefined)}
              size="md"
              error={attemptedSubmit && missingTipo}
              helperText={
                <RequiredTooltip
                  visible={attemptedSubmit && missingTipo}
                  inline
                  message="Debe seleccionar el tipo de respuesta."
                />
              }
            />
          </div>

          <div className={styles.infoBox}>
            <div className={styles.infoMeta}>
              <div className={styles.infoItem}>
                <span className={styles.infoLabel}>Radicado</span>
                <strong className={styles.infoValue}>{radicado}</strong>
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
          variant="email"
          placeholder="correo@dominio.com"
          value={etiquetas}
          error={attemptedSubmit && missingCorreos}
          helperText={
            <RequiredTooltip
              visible={attemptedSubmit && missingCorreos}
              inline
              message="Debe informar al menos un correo."
            />
          }
          onAddTag={(tag) =>
            setEtiquetas((current) => (current.includes(tag) ? current : [...current, tag]))
          }
          onRemoveTag={(tag) =>
            setEtiquetas((current) => current.filter((currentTag) => currentTag !== tag))
          }
          onRemoveAll={() => setEtiquetas([])}
        />
      </div>
      </AppModal>

      <ConfirmacionEnvioModal
        open={isConfirmacionOpen}
        onClose={() => setIsConfirmacionOpen(false)}
        radicado={radicado}
        fechaEnvio={new Date().toLocaleDateString("es-CO")}
        destinatario={destinatario}
        correoEnviado={correoConfirmado || "-"}
      />
    </>
  );
}
