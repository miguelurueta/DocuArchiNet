import { CarryOutFilled, MailFilled } from "@ant-design/icons";
import { useCallback, useId, useMemo, useState, useSyncExternalStore } from "react";
import {
  AppEditor,
  AppEditorSaveAction,
  useAppEditorSaveState,
} from "../../../../app/Components/UI/AppEditor";
import { AppSteps, type AppStepItem } from "../../../../app/Components/UI/AppSteps";
import { AppToolbar } from "../../../../app/Components/UI/AppToolbar";
import type { AppUploadFile } from "../../../../app/Components/UI/AppUpload/AppUpload";
import { AppUpload } from "../../../../app/Components/UI/AppUpload/AppUpload";
import styles from "./GestionRespuestaMainTabContent.module.css";
import { GestionRespuestaEditorContainer } from "./GestionRespuestaEditorContainer";
import { GestionRespuestaRightToolsPanel } from "./GestionRespuestaRightToolsPanel";
import { GestionDocumentoModal } from "./modalGestionDocumento";

const DEFAULT_MEDIA_QUERY = "(max-width: 1024px)";
const MOBILE_MEDIA_QUERY = "(max-width: 768px)";

const useMediaQuery = (query: string) => {
  const getSnapshot = () =>
    typeof window !== "undefined" ? window.matchMedia(query).matches : false;

  const subscribe = (onStoreChange: () => void) => {
    if (typeof window === "undefined") {
      return () => undefined;
    }

    const mediaQueryList = window.matchMedia(query);
    const onChange = () => onStoreChange();

    mediaQueryList.addEventListener("change", onChange);
    return () => {
      mediaQueryList.removeEventListener("change", onChange);
    };
  };

  return useSyncExternalStore(subscribe, getSnapshot, () => false);
};

type GestionRespuestaMainTabContentProps = {
  idTareaWf?: number;
};

export function GestionRespuestaMainTabContent(
  _props: GestionRespuestaMainTabContentProps = {},
) {
  const panelId = useId();
  const isCompact = useMediaQuery(DEFAULT_MEDIA_QUERY);
  const isMobile = useMediaQuery(MOBILE_MEDIA_QUERY);
  const [isPanelCollapsed, setIsPanelCollapsed] = useState(isCompact);
  const [isGestionDocumentoModalOpen, setIsGestionDocumentoModalOpen] =
    useState(false);
  const [files, setFiles] = useState<AppUploadFile[]>([]);
  const [currentStep, setCurrentStep] = useState(0);
  const [editorValue, setEditorValue] = useState<string>("");
  const [savedEditorValue, setSavedEditorValue] = useState<string>("");
  const { saveStatus } = useAppEditorSaveState({
    currentValue: editorValue,
    savedValue: savedEditorValue,
  });
  const canAdvanceToSend = files.length > 0;
  // Visual guides are managed by AppEditorPdf; this view intentionally stays on AppEditor.
  const editorPageMargins = useMemo(
    () => ({ top: 96, right: 72, bottom: 96, left: 72 }),
    [],
  );

  const stepItems = useMemo<AppStepItem[]>(
    () => [
      {
        key: "redaccion",
        title: "Redaccion",
        description: "Construye el contenido de la respuesta",
        status: currentStep > 0 ? "finish" : "process",
      },
      {
        key: "adjuntos",
        title: "Adjuntos",
        description: canAdvanceToSend
          ? `${files.length} archivo(s) listo(s) para envio`
          : "Adjunta al menos un archivo para continuar",
        status: currentStep > 1 ? "finish" : currentStep === 1 ? "process" : "wait",
      },
      {
        key: "envio",
        title: "Envio",
        description: "Confirma y finaliza el envio",
        status: currentStep === 2 ? "process" : "wait",
      },
    ],
    [canAdvanceToSend, currentStep, files.length],
  );

  const goToSendStep = useCallback(() => {
    if (!canAdvanceToSend) {
      setCurrentStep(1);
      return;
    }
    setCurrentStep(2);
    setIsGestionDocumentoModalOpen(true);
  }, [canAdvanceToSend]);

  const handleStepChange = useCallback(
    (nextStep: number) => {
      if (nextStep === 2) {
        goToSendStep();
        return;
      }
      setCurrentStep(nextStep);
    },
    [goToSendStep],
  );

  const validateStep = useCallback(
    (stepIndex: number) => {
      if (stepIndex === 1) {
        return canAdvanceToSend;
      }
      return true;
    },
    [canAdvanceToSend],
  );
  // NOTE: Page context/metrics tracking is intentionally owned by AppEditorPdf.
  // This consumer stays decoupled from AppEditorPdf and only uses AppEditor as engine.

  return (
    <section className={styles.mainTab} aria-label="Contenido principal de respuesta">
      <div className={styles.workbench}>
        <div className={styles.workflowSteps}>
          <AppSteps
            items={stepItems}
            variant="form"
            size="sm"
            current={currentStep}
            onChange={handleStepChange}
            validateStep={validateStep}
          />
          {!canAdvanceToSend ? (
            <p className={styles.workflowHint}>
              Para habilitar envio, carga al menos un archivo en el bloque de adjuntos.
            </p>
          ) : null}
        </div>

        <AppToolbar
          className={styles.toolbar}
          actions={[
            {
              key: "solicitud-aprobacion",
              label: "Solicitud de Aprobacion",
              size: "sm",
              variant: "ghost",
              icon: <CarryOutFilled />,
            },
            {
              key: "enviar",
              label: "Enviar",
              size: "sm",
              variant: "ghost",
              icon: <MailFilled />,
              onClick: () => goToSendStep(),
            },
          ]}
        />

        <div
          className={styles.workbenchBody}
          data-panel-collapsed={isPanelCollapsed}
          data-variant={isMobile ? "overlay" : "inline"}
          data-testid="gestion-respuesta-workbench"
        >
          <GestionRespuestaEditorContainer>
            <AppEditor
              label="Editor principal de respuesta"
              value={editorValue}
              onChange={setEditorValue}
              toolbarActions={
                <AppEditorSaveAction
                  iconOnly
                  saveStatus={saveStatus}
                  onSave={() => {
                    setSavedEditorValue(editorValue);
                  }}
                />
              }
              placeholder="Escribe aqui la respuesta principal..."
              aria-label="Contenido del editor principal de respuesta"
              className={styles.embeddedAppEditor}
              surfaceClassName={styles.embeddedAppEditorSurface}
              minHeight="100%"
              paginationMode="visual"
              pageFormat="A4"
              pageOrientation="portrait"
              pageMargins={editorPageMargins}
            />
          </GestionRespuestaEditorContainer>
          <GestionRespuestaRightToolsPanel
            collapsed={isPanelCollapsed}
            panelId={panelId}
            onToggle={() => setIsPanelCollapsed((prev) => !prev)}
          />
        </div>
      </div>

      <div className={styles.attachments}>
        <div className={styles.attachmentsHeader}>
          <h3 className={styles.attachmentsTitle}>Adjuntos</h3>
          <span className={styles.infoCopy}>Carga de soportes y anexos del expediente.</span>
        </div>
        <AppUpload value={files} onChange={setFiles} drag size="sm" />
      </div>

      <GestionDocumentoModal
        open={isGestionDocumentoModalOpen}
        onClose={() => {
          setIsGestionDocumentoModalOpen(false);
          if (currentStep === 2) {
            setCurrentStep(1);
          }
        }}
      />
    </section>
  );
}
