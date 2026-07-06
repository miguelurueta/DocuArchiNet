import { Alert } from "antd";
import { useCallback, useMemo, useState } from "react";
import { AppUploadDocumental } from "../../../almacenamientoDocumental/components/AppUploadDocumental";
import type {
  AlmacenarDocumentoStoredResult,
  UploadDocumentalBatchSummary,
  UploadDocumentalContext,
  UploadDocumentalStoredContext,
} from "../../../almacenamientoDocumental/components/AppUploadDocumental";
import {
  buildGestionRespuestaAlmacenarDocumentoRequest,
  isWorkflowAnexoCreated,
} from "../../adapters/gestionRespuestaUploadDocumental.mapper";
import { obtenerUsuarioIdAutenticado } from "../../../../app/auth/Infraestructura/ManejadorJWT";
import { useEmpresaActual } from "../../../login/hooks/useEmpresaActual";
import {
  loadGestionRespuestaTiposDocumentales,
  loadGestionRespuestaUploadConfig,
} from "../../services/gestionRespuestaUploadDocumental.service";
import { useGestionRespuestaDocumentos } from "../../hooks/useGestionRespuestaDocumentos";
import styles from "./GestionRespuestaMainTabContent.module.css";

const PROCESO_GESTION_RESPUESTA_ANEXO = "gestion-respuesta-anexo";
const GESTION_RESPUESTA_MAX_CHUNK_SIZE_BYTES = 4 * 1024 * 1024;
const DEBUG_GESTION_RESPUESTA_UPLOAD_DOCUMENTAL =
  typeof import.meta !== "undefined" &&
  Boolean(import.meta.env?.DEV) &&
  import.meta.env?.MODE !== "test";

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
  const {
    empresa,
    isLoading: empresaLoading,
    isError: empresaError,
  } = useEmpresaActual();
  const [uploadError, setUploadError] = useState<string | null>(null);
  const idUsuarioGestion = obtenerUsuarioIdAutenticado();
  const idEmpresa = normalizePositiveNumber(empresa?.IdEmpresa);

  const uploadContext = useMemo<UploadDocumentalContext>(
    () => ({
      nombreGabinete: nombreGabinete ?? "",
      idTareaWorkflow: idTareaWf,
      idRutaWorkflow: idRutaWf,
      idRespuesta: normalizeIdRespuesta(idRespuestaRadicado),
      nameModulo: radicado,
      idUsuarioGestion,
      idEmpresa,
      fechaElaboracion: getCurrentDateOnly(),
    }),
    [idEmpresa, idRespuestaRadicado, idRutaWf, idTareaWf, idUsuarioGestion, nombreGabinete, radicado],
  );

  const refreshDocumentosAfterStore = useCallback(() => {
    debugGestionRespuestaUploadDocumental("refresh after store", {
      reason: "stored-document-confirmed",
    });
    refreshDocumentos();
  }, [refreshDocumentos]);

  const handleStored = useCallback(
    (result: AlmacenarDocumentoStoredResult, storedContext: UploadDocumentalStoredContext) => {
      const anexoCreated = isWorkflowAnexoCreated(result.rawBackendResult);
      debugGestionRespuestaUploadDocumental("stored", {
        source: storedContext.source,
        fileUid: result.fileUid,
        fileName: result.fileName,
        idAlmacen: result.idAlmacen,
        idRegistroProduccionDocumental: result.idRegistroProduccionDocumental,
        requestId: result.requestId,
        anexoCreated,
        remainingFiles: storedContext.remainingFiles,
      });

      if (anexoCreated && storedContext.source === "single") {
        refreshDocumentosAfterStore();
        if (storedContext.remainingFiles === 0) {
          onClose?.();
        }
      }
    },
    [onClose, refreshDocumentosAfterStore],
  );

  const handleBatchComplete = useCallback(
    (summary: UploadDocumentalBatchSummary) => {
      debugGestionRespuestaUploadDocumental("batch complete", {
        total: summary.total,
        stored: summary.stored,
        failed: summary.failed,
        skipped: summary.skipped,
        cancelled: summary.cancelled,
        remainingFiles: summary.remainingFiles,
      });
      if (summary.stored > 0) {
        refreshDocumentosAfterStore();
      }

      if (
        summary.stored > 0 &&
        summary.failed === 0 &&
        summary.skipped === 0 &&
        summary.cancelled === 0 &&
        summary.remainingFiles === 0
      ) {
        onClose?.();
      }
    },
    [onClose, refreshDocumentosAfterStore],
  );

  const handleError = useCallback((error: unknown) => {
    if (isTipologiaRequiredError(error)) {
      setUploadError(null);
      return;
    }

    setUploadError(error instanceof Error ? error.message : "No fue posible almacenar el anexo.");
  }, []);

  if (gabineteLoading) {
    return <Alert type="info" showIcon title="Cargando contexto documental..." />;
  }

  if (empresaLoading) {
    return <Alert type="info" showIcon title="Cargando empresa para inventario documental..." />;
  }

  if (gabineteError) {
    return <Alert type="error" showIcon title={gabineteError} />;
  }

  if (empresaError || !idEmpresa) {
    return <Alert type="error" showIcon title="No fue posible resolver la empresa para el inventario documental." />;
  }

  if (!idUsuarioGestion) {
    return <Alert type="error" showIcon title="No fue posible resolver el usuario para el inventario documental." />;
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
          title={uploadError}
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
        saveAllMode="inline"
        validationMode="queue-with-error"
        tipologiaObligatoria
        autoSuggestTipologia
        loadConfig={loadGestionRespuestaUploadConfig}
        loadTiposDocumentales={loadGestionRespuestaTiposDocumentales}
        buildStoreRequest={buildGestionRespuestaAlmacenarDocumentoRequest}
        storageOptions={{
          backendPayloadCase: "pascal",
          validateStatusBeforeComplete: true,
          maxChunkSizeBytes: GESTION_RESPUESTA_MAX_CHUNK_SIZE_BYTES,
        }}
        onStored={handleStored}
        onBatchComplete={handleBatchComplete}
        onError={handleError}
      />
    </div>
  );
}

function normalizeIdRespuesta(value: string | number | undefined): number | undefined {
  const normalized = typeof value === "string" ? Number(value) : value;
  return typeof normalized === "number" && Number.isFinite(normalized) && normalized > 0 ? normalized : undefined;
}

function normalizePositiveNumber(value: unknown): number | undefined {
  const normalized = typeof value === "string" ? Number(value) : value;
  return typeof normalized === "number" && Number.isFinite(normalized) && normalized > 0 ? normalized : undefined;
}

function getCurrentDateOnly(): string {
  return new Date().toISOString().slice(0, 10);
}

function debugGestionRespuestaUploadDocumental(message: string, payload?: Record<string, unknown>): void {
  if (!DEBUG_GESTION_RESPUESTA_UPLOAD_DOCUMENTAL) {
    return;
  }

  console.info(`[GestionRespuestaUploadDocumental][debug] ${message}`, payload ?? {});
}

function isTipologiaRequiredError(error: unknown): boolean {
  const message = error instanceof Error ? error.message : typeof error === "string" ? error : "";
  const normalized = message
    .normalize("NFD")
    .replace(/[\u0300-\u036f]/g, "")
    .toLowerCase();

  return (
    normalized.includes("selecciona la tipologia") ||
    normalized.includes("tipologia documental") ||
    normalized.includes("tipo documental")
  );
}
