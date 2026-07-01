import { Alert } from "antd";
import { useCallback, useMemo, useState } from "react";
import { AppUploadDocumental } from "../../../almacenamientoDocumental/components/AppUploadDocumental";
import type {
  AlmacenarDocumentoStoredResult,
  UploadDocumentalContext,
} from "../../../almacenamientoDocumental/components/AppUploadDocumental";
import {
  buildGestionRespuestaAlmacenarDocumentoRequest,
  isWorkflowAnexoCreated,
} from "../../adapters/gestionRespuestaUploadDocumental.mapper";
import {
  loadGestionRespuestaTiposDocumentales,
  loadGestionRespuestaUploadConfig,
} from "../../services/gestionRespuestaUploadDocumental.service";
import { useGestionRespuestaDocumentos } from "../../hooks/useGestionRespuestaDocumentos";
import styles from "./GestionRespuestaMainTabContent.module.css";

const PROCESO_GESTION_RESPUESTA_ANEXO = "gestion-respuesta-anexo";

export type GestionRespuestaUploadDocumentalProps = {
  embedded?: boolean;
  open?: boolean;
  onClose?: () => void;
};

export function GestionRespuestaUploadDocumental({
  embedded = true,
  open = true,
  onClose,
}: GestionRespuestaUploadDocumentalProps = {}) {
  const {
    idTareaWf,
    idRutaWf,
    radicado,
    idRespuestaRadicado,
    nombreGabinete,
    gabineteLoading,
    gabineteError,
    refreshDocumentos,
  } = useGestionRespuestaDocumentos();
  const [uploadError, setUploadError] = useState<string | null>(null);

  const uploadContext = useMemo<UploadDocumentalContext>(
    () => ({
      nombreGabinete: nombreGabinete ?? "",
      idTareaWorkflow: idTareaWf,
      idRutaWorkflow: idRutaWf,
      idRespuesta: normalizeIdRespuesta(idRespuestaRadicado),
      nameModulo: radicado,
    }),
    [idRespuestaRadicado, idRutaWf, idTareaWf, nombreGabinete, radicado],
  );

  const handleStored = useCallback(
    (result: AlmacenarDocumentoStoredResult) => {
      if (isWorkflowAnexoCreated(result.rawBackendResult)) {
        refreshDocumentos();
      }
    },
    [refreshDocumentos],
  );

  const handleError = useCallback((error: unknown) => {
    setUploadError(error instanceof Error ? error.message : "No fue posible almacenar el anexo.");
  }, []);

  if (gabineteLoading) {
    return <Alert type="info" showIcon title="Cargando contexto documental..." />;
  }

  if (gabineteError) {
    return <Alert type="error" showIcon title={gabineteError} />;
  }

  if (!nombreGabinete) {
    return <Alert type="warning" showIcon title="No hay gabinete documental disponible para cargar anexos." />;
  }

  if (!uploadContext.idRespuesta) {
    return <Alert type="warning" showIcon title="No hay respuesta de radicado disponible para asociar anexos." />;
  }

  if (!uploadContext.idRutaWorkflow) {
    return <Alert type="warning" showIcon title="No hay ruta workflow disponible para cargar tipologias documentales." />;
  }

  return (
    <div className={styles.documentalUploadAdapter}>
      {uploadError ? (
        <Alert
          type="error"
          closable
          message={uploadError}
          onClose={() => setUploadError(null)}
          className={styles.documentalUploadAlert}
        />
      ) : null}

      <AppUploadDocumental
        proceso={PROCESO_GESTION_RESPUESTA_ANEXO}
        context={uploadContext}
        title="Adjuntos"
        embedded={embedded}
        open={open}
        onClose={onClose}
        allowSingleFileStore
        validationMode="queue-with-error"
        tipologiaObligatoria
        autoSuggestTipologia
        loadConfig={loadGestionRespuestaUploadConfig}
        loadTiposDocumentales={loadGestionRespuestaTiposDocumentales}
        buildStoreRequest={buildGestionRespuestaAlmacenarDocumentoRequest}
        storageOptions={{
          backendPayloadCase: "pascal",
          validateStatusBeforeComplete: true,
        }}
        onStored={handleStored}
        onError={handleError}
      />
    </div>
  );
}

function normalizeIdRespuesta(value: string | number | undefined): number | undefined {
  const normalized = typeof value === "string" ? Number(value) : value;
  return typeof normalized === "number" && Number.isFinite(normalized) && normalized > 0 ? normalized : undefined;
}
