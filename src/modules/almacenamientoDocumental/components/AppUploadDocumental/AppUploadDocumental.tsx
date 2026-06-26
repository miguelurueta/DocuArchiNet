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
  const actions = useAppUploadDocumentalActions({
    files: state.files,
    config: state.config,
    context: props.context,
    proceso: props.proceso,
    modoDocumento: props.modoDocumento,
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
      state.tiposDocumentales.map((tipo) => ({
        label: tipo.nombreTipoDocumento,
        value: tipo.idTipoDocumento,
      })),
    [state.tiposDocumentales],
  );

  const requiresTypology = Boolean(tipologiaObligatoria ?? state.config?.requiereTipologia);
  const shouldRenderDate = Boolean(requiereFechaCarga ?? state.config?.requiereFechaCarga);
  const isDateRequired = Boolean(fechaCargaObligatoria ?? state.config?.fechaCargaObligatoria ?? shouldRenderDate);

  const handleTypologyChange = useCallback(
    (uid: string, value: number | number[] | undefined) => {
      const selectedValue = Array.isArray(value) ? value[0] : value;
      const selected = state.tiposDocumentales.find((tipo) => tipo.idTipoDocumento === selectedValue);
      state.updateMetadata(
        uid,
        {
          idTipoDocumento: selected?.idTipoDocumento,
          nombreTipoDocumento: selected?.nombreTipoDocumento,
          suggestionConfidence: undefined,
        },
        true,
      );
    },
    [state],
  );

  const renderMetadata = useCallback(
    ({ item, disabled }: { item: AppUploadBatchFileItem<UploadDocumentalFileMetadata>; disabled: boolean }) => {
      const metadata = item.metadata ?? {};

      return (
        <div className={styles.metadataGrid}>
          {requiresTypology ? (
            <AppInputSelect<number>
              value={metadata.idTipoDocumento}
              options={tipoOptions}
              size="sm"
              placeholder="Tipologia"
              disabled={disabled || state.tiposDocumentales.length === 0}
              allowClear
              searchable
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
                state.updateMetadata(item.uid, { fechaCarga: event.currentTarget.value || undefined })
              }
            />
          ) : null}

          {metadata.warning ? <p className={styles.warningText}>{metadata.warning}</p> : null}
          {metadata.error ? <p className={styles.errorText}>{metadata.error}</p> : null}
        </div>
      );
    },
    [
      autoSuggestTipologia,
      handleTypologyChange,
      isDateRequired,
      requiresTypology,
      shouldRenderDate,
      state,
      tipoOptions,
    ],
  );

  if (!embedded && !open) {
    return null;
  }

  return (
    <div className={styles.root} data-embedded={embedded ? "true" : "false"}>
      {state.loaderError ? (
        <Alert className={styles.alert} type="error" showIcon title={state.loaderError} />
      ) : null}

      <AppUploadBatchView<UploadDocumentalFileMetadata>
        title={title}
        description="Carga documental por archivo con tipologia y registro individual."
        files={state.files}
        selectedUid={state.selectedUid}
        accept={state.config?.accept}
        maxSize={state.config?.maxSizeBytes}
        multiple={state.config?.multiple ?? true}
        drag
        disabled={state.selectionDisabled}
        loading={state.loading}
        canAddFiles={!state.selectionDisabled}
        canSaveAll={actions.canSaveAll}
        canSaveOne={allowSingleFileStore}
        canClearAll={state.files.length > 0}
        summary={state.summary}
        emptyMessage="No hay documentos en la cola."
        onFilesSelected={state.handleFilesSelected}
        onSelectFile={state.setSelectedUid}
        onPreviewFile={state.setSelectedUid}
        onRemoveFile={state.removeFile}
        onClearAll={state.clearFiles}
        onSaveFile={(uid) => void actions.saveOne(uid)}
        onSaveAll={actions.saveAll}
        onClosePreview={() => state.setSelectedUid(undefined)}
        renderMetadata={renderMetadata}
        renderFooterExtra={(summary) => (
          <div className={styles.footerCounters}>
            <span>Pendientes: {summary.ready + summary.queued}</span>
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
