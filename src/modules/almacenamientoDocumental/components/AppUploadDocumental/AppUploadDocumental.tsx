import { Alert } from "antd";
import { useCallback, useMemo } from "react";
import { AppInput } from "../../../../app/Components/UI/AppInput";
import { AppInputSelect } from "../../../../app/Components/UI/AppInputSelect";
import type { AppInputSelectOption } from "../../../../app/Components/UI/AppInputSelect";
import { AppProgressBatch } from "../../../../app/Components/UI/AppProgressBatch";
import { AppUploadBatchView } from "../../../../app/Components/UI/AppUploadBatchView";
import type { AppUploadBatchFileItem } from "../../../../app/Components/UI/AppUploadBatchView";
import { useAppUploadDocumentalActions } from "./hooks/useAppUploadDocumentalActions";
import { useAppUploadDocumentalState } from "./hooks/useAppUploadDocumentalState";
import type {
  AppUploadDocumentalProps,
  TipoDocumentalOption,
  UploadDocumentalFileMetadata,
} from "./AppUploadDocumental.types";
import styles from "./AppUploadDocumental.module.css";

const DEFAULT_TITLE = "Adjuntar documentos";

export function AppUploadDocumental(props: AppUploadDocumentalProps) {
  const {
    title = DEFAULT_TITLE,
    embedded = true,
    open = true,
    allowSingleFileStore = true,
    autoSuggestTipologia = true,
    requiereFechaCarga,
    fechaCargaObligatoria,
    tipologiaObligatoria,
    onClose,
  } = props;

  const state = useAppUploadDocumentalState(props);
  const {
    files,
    selectedUid,
    config,
    tiposDocumentales,
    loading,
    loaderError,
    selectionDisabled,
    summary,
    handleFilesSelected,
    setSelectedUid,
    updateMetadata,
    removeFile,
    clearFiles,
  } = state;
  const actions = useAppUploadDocumentalActions({
    files,
    config,
    context: props.context,
    proceso: props.proceso,
    modoDocumento: props.modoDocumento,
    buildStoreRequest: props.buildStoreRequest,
    storageOptions: props.storageOptions,
    operationId: state.operationId,
    validateFileForStore: state.validateFileForStore,
    markFile: state.markFile,
    onStored: props.onStored,
    onInterfaceRegistration: props.onInterfaceRegistration,
    onBatchComplete: props.onBatchComplete,
    onError: props.onError,
  });

  const tipoOptions = useMemo<AppInputSelectOption<number>[]>(
    () =>
      tiposDocumentales.map((tipo) => ({
        label: tipo.nombreTipoDocumento,
        value: tipo.idTipoDocumento,
      })),
    [tiposDocumentales],
  );

  const requiresTypology = Boolean(tipologiaObligatoria ?? config?.requiereTipologia);
  const shouldRenderDate = Boolean(requiereFechaCarga ?? config?.requiereFechaCarga);
  const isDateRequired = Boolean(fechaCargaObligatoria ?? config?.fechaCargaObligatoria ?? shouldRenderDate);

  const handleTypologyChange = useCallback(
    (uid: string, value: number | number[] | undefined) => {
      const selectedValue = Array.isArray(value) ? value[0] : value;
      const selected = tiposDocumentales.find((tipo) => tipo.idTipoDocumento === selectedValue);
      updateMetadata(
        uid,
        {
          idTipoDocumento: selected?.idTipoDocumento,
          nombreTipoDocumento: selected?.nombreTipoDocumento,
          suggestionConfidence: undefined,
        },
        true,
      );
    },
    [tiposDocumentales, updateMetadata],
  );

  const renderMetadata = useCallback(
    ({
      item,
      disabled,
    }: {
      item: AppUploadBatchFileItem<UploadDocumentalFileMetadata>;
      disabled: boolean;
    }) => {
      const metadata = item.metadata ?? {};

      return (
        <div className={styles.metadataGrid}>
          {requiresTypology ? (
            <AppInputSelect<number>
              value={metadata.idTipoDocumento}
              options={tipoOptions}
              size="sm"
              placeholder="Tipologia"
              disabled={disabled || tiposDocumentales.length === 0}
              allowClear
              label="Tipologia"
              error={Boolean(metadata.error && !metadata.idTipoDocumento)}
              helperText={
                metadata.suggestionConfidence && autoSuggestTipologia
                  ? `Sugerida ${Math.round(metadata.suggestionConfidence * 100)}%`
                  : undefined
              }
              aria-label={`Tipologia de ${item.name}`}
              onChange={(value) => handleTypologyChange(item.uid, value)}
            />
          ) : null}

          {shouldRenderDate ? (
            <AppInput
              type="date"
              label="Fecha"
              value={metadata.fechaCarga ?? ""}
              disabled={disabled}
              error={Boolean(metadata.error && isDateRequired && !metadata.fechaCarga)}
              aria-label={`Fecha documental de ${item.name}`}
              onChange={(event) =>
                updateMetadata(item.uid, { fechaCarga: event.currentTarget.value || undefined })
              }
            />
          ) : null}

          {metadata.warning ? <p className={styles.warningText}>{metadata.warning}</p> : null}
        </div>
      );
    },
    [
      autoSuggestTipologia,
      handleTypologyChange,
      isDateRequired,
      requiresTypology,
      shouldRenderDate,
      tiposDocumentales.length,
      tipoOptions,
      updateMetadata,
    ],
  );

  if (!embedded && !open) {
    return null;
  }

  return (
    <div className={styles.root} data-embedded={embedded ? "true" : "false"}>
      {loaderError ? (
        <Alert className={styles.alert} type="error" showIcon title={loaderError} />
      ) : null}

      <AppUploadBatchView<UploadDocumentalFileMetadata>
        title={title}
        description="Carga documental por archivo con tipologia y registro individual."
        files={files}
        selectedUid={selectedUid}
        accept={config?.accept}
        maxSize={config?.maxSizeBytes}
        multiple={config?.multiple ?? true}
        drag
        disabled={selectionDisabled}
        loading={loading}
        canAddFiles={!selectionDisabled}
        canSaveAll={actions.canSaveAll}
        canSaveOne={allowSingleFileStore}
        canClearAll={files.length > 0}
        summary={summary}
        emptyMessage="No hay documentos en la cola."
        onFilesSelected={handleFilesSelected}
        onSelectFile={setSelectedUid}
        onPreviewFile={setSelectedUid}
        onRemoveFile={removeFile}
        onClearAll={clearFiles}
        onSaveFile={(uid) => void actions.saveOne(uid)}
        onSaveAll={actions.saveAll}
        onClosePreview={() => setSelectedUid(undefined)}
        renderMetadata={renderMetadata}
        renderFooterExtra={(summary) => (
          <div className={styles.footerCounters}>
            <span>Archivos: {summary.ready + summary.queued}</span>
            <span>Errores: {summary.error}</span>
            <span>Almacenados: {summary.done}</span>
          </div>
        )}
      />

      <AppProgressBatch
        open={actions.batchOpen}
        items={actions.batchItems}
        title="Guardar documentos"
        processName="Almacenamiento documental"
        autoStart
        closeOnComplete={false}
        confirmOnCancel
        onOpenChange={(nextOpen) => {
          actions.setBatchOpen(nextOpen);
          if (!nextOpen && !embedded) {
            onClose?.();
          }
        }}
        getItemLabel={(item) => item.name}
        processItem={actions.processBatchItem}
        onComplete={actions.handleBatchComplete}
        onCancel={actions.handleBatchComplete}
        onError={props.onError}
      />
    </div>
  );
}

export function toTipoDocumentalOption(tipo: TipoDocumentalOption): AppInputSelectOption<number> {
  return {
    label: tipo.nombreTipoDocumento,
    value: tipo.idTipoDocumento,
  };
}
