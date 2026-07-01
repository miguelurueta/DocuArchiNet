import { CarryOutFilled, MailFilled } from "@ant-design/icons";
import { useCallback, useState } from "react";
import {
  AppEditor,
  AppEditorSaveAction,
  useAppEditorSaveState,
} from "../../../../app/Components/UI/AppEditor";
import { AppToolbar } from "../../../../app/Components/UI/AppToolbar";
import { useGestionRespuestaDocumentos } from "../../hooks/useGestionRespuestaDocumentos";
import styles from "./GestionRespuestaMainTabContent.module.css";
import { GestionRespuestaEditorContainer } from "./GestionRespuestaEditorContainer";
import { GestionRespuestaUploadDocumentalModal } from "./GestionRespuestaUploadDocumentalModal";
import { GestionDocumentoModal } from "./modalGestionDocumento";

type GestionRespuestaMainTabContentProps = {
  idTareaWf?: number;
};

export function GestionRespuestaMainTabContent(
  _props: GestionRespuestaMainTabContentProps = {},
) {
  void _props;
  const [isGestionDocumentoModalOpen, setIsGestionDocumentoModalOpen] =
    useState(false);
  const { nombreGabinete, idRespuestaRadicado } = useGestionRespuestaDocumentos();
  const [editorValue, setEditorValue] = useState<string>("");
  const [savedEditorValue, setSavedEditorValue] = useState<string>("");
  const { saveStatus } = useAppEditorSaveState({
    currentValue: editorValue,
    savedValue: savedEditorValue,
  });
  const canAdvanceToSend = Boolean(nombreGabinete && idRespuestaRadicado);
  const goToSendStep = useCallback(() => {
    if (!canAdvanceToSend) {
      return;
    }
    setIsGestionDocumentoModalOpen(true);
  }, [canAdvanceToSend]);

  return (
    <section
      className={styles.mainTab}
      aria-label="Contenido principal de respuesta"
    >
      <div className={styles.workbench}>
        <AppToolbar
          className={styles.toolbar}
          density="compact"
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
          data-testid="gestion-respuesta-workbench"
        >
          <GestionRespuestaEditorContainer>
            <AppEditor
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
            />
          </GestionRespuestaEditorContainer>
        </div>
      </div>

      <div className={styles.attachments}>
        <GestionRespuestaUploadDocumentalModal />
      </div>

      <GestionDocumentoModal
        open={isGestionDocumentoModalOpen}
        onClose={() => {
          setIsGestionDocumentoModalOpen(false);
        }}
      />
    </section>
  );
}
