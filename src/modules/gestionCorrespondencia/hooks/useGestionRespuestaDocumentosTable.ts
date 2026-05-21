import { useCallback, useMemo, useRef, useState } from "react";
import type {
  AppTreeTableLoadChildrenResult,
  AppTreeTableLoadResult,
  AppTreeTableRow,
} from "../../../app/Components/UI/AppTreeTable";
import { actionListaDocumentosRadicados, queryListaDocumentosRadicados, resolveDocumentoVisualizacion } from "../services/listaDocumentosRadicados.service";
import { getSolicitaGabinetePorTareaWorkflow } from "../services/solicitaGabineteRadicadoWorkflow.service";
import { buildListaDocumentosRadicadosActionRequest } from "../adapters/documentosWorkbenchActionMapper";
import { adaptListaDocumentosRadicadosToWorkbenchModel } from "../adapters/documentosWorkbenchResponseAdapter";
import {
  buildListaDocumentosRadicadosChildrenQuery,
  buildListaDocumentosRadicadosRootQuery,
} from "../adapters/gestionRespuestaDocumentosRequestMapper";
import type { ListaDocumentosRadicadosRowDto } from "../types/listaDocumentosRadicados.types";

const TABLE_ID = "InboxListaRadicados";

const readString = (record: unknown, ...keys: string[]): string | undefined => {
  if (!record || typeof record !== "object") return undefined;
  const source = record as Record<string, unknown>;
  for (const key of keys) {
    const value = source[key];
    if (typeof value === "string" && value.trim().length > 0) return value.trim();
  }
  return undefined;
};

const readNumber = (record: unknown, ...keys: string[]): number | undefined => {
  if (!record || typeof record !== "object") return undefined;
  const source = record as Record<string, unknown>;
  for (const key of keys) {
    const value = source[key];
    if (typeof value === "number" && Number.isFinite(value)) return value;
  }
  return undefined;
};

const resolveFileUrlFromResolveResponse = (input: unknown): string | undefined => {
  if (!input || typeof input !== "object") return undefined;
  const obj = input as Record<string, unknown>;
  return (
    readString(obj, "fileUrl", "FileUrl", "url", "Url") ??
    readString(obj.data, "fileUrl", "FileUrl", "url", "Url") ??
    readString(obj.meta, "fileUrl", "FileUrl", "url", "Url")
  );
};

export type GestionRespuestaDocumentoActivo = {
  fileUrl: string;
  documentId?: number;
  nombreGabinete?: string;
  rowId: string;
};

export const useGestionRespuestaDocumentosTable = (idTareaWf?: number) => {
  const latestRowRef = useRef<Map<string, ListaDocumentosRadicadosRowDto>>(new Map());
  const gabineteRef = useRef<string | undefined>(undefined);
  const [tableColumns, setTableColumns] = useState<import("ag-grid-community").ColDef<Record<string, unknown>>[]>();

  const load = useCallback(async (): Promise<AppTreeTableLoadResult> => {
    try {
      let gabinete: string | undefined;
      if (typeof idTareaWf === "number" && Number.isFinite(idTareaWf) && idTareaWf > 0) {
        const gabineteResponse = await getSolicitaGabinetePorTareaWorkflow(idTareaWf);
        if (!gabineteResponse.success) {
          const message =
            gabineteResponse.errors?.[0]?.errorMessage ??
            gabineteResponse.message ??
            "No fue posible resolver el gabinete del radicado.";
          return { ok: false, message };
        }

        gabinete = gabineteResponse.data?.NombreGabinete;
      }

      gabineteRef.current = gabinete;
      const response = await queryListaDocumentosRadicados(buildListaDocumentosRadicadosRootQuery({ idTareaWf, nombreGabinete: gabinete }));
      if (!response.success || !response.data) {
        const message =
          response.errors?.[0]?.errorMessage ?? response.message ?? "No fue posible cargar el listado.";
        return { ok: false, message };
      }

      const model = adaptListaDocumentosRadicadosToWorkbenchModel(response.data);
      latestRowRef.current = new Map((response.data.Rows ?? []).map((row) => [row.RowId, row]));
      setTableColumns(model.tableColumns);
      return { ok: true, rows: model.rows };
    } catch {
      return { ok: false, message: "No fue posible cargar el listado." };
    }
  }, [idTareaWf]);

  const loadChildren = useCallback(
    async (row: AppTreeTableRow): Promise<AppTreeTableLoadChildrenResult> => {
      const parentNodeType = String(row.meta?.NodeType ?? row.meta?.nodeType ?? "");
      const gabinete = gabineteRef.current;

      try {
        const response = await queryListaDocumentosRadicados(
          buildListaDocumentosRadicadosChildrenQuery({
            nombreGabinete: gabinete,
            parentRowId: row.id,
            parentNodeType: parentNodeType || null,
            level: Number(row.meta?.Level ?? 2),
          }),
        );
        if (!response.success || !response.data) {
          const message =
            response.errors?.[0]?.errorMessage ?? response.message ?? "No fue posible cargar el listado.";
          return { ok: false, message };
        }

        const model = adaptListaDocumentosRadicadosToWorkbenchModel(response.data);
        for (const childRow of response.data.Rows ?? []) {
          latestRowRef.current.set(childRow.RowId, childRow);
        }
        return { ok: true, rows: model.rows };
      } catch {
        return { ok: false, message: "No fue posible cargar el listado." };
      }
    },
    [],
  );

  const performVerDocumento = useCallback(async (rowId: string): Promise<GestionRespuestaDocumentoActivo | null> => {
    const selected = latestRowRef.current.get(rowId);
    const meta = selected?.Meta;
    const values = selected?.Values;

    const nodeType = readString(meta, "NodeType", "nodeType") ?? "documento";
    const documentIdFromMeta = readNumber(meta, "DocumentId", "documentId");
    const gabinete =
      readString(meta, "NombreGabinete", "nombreGabinete", "NOMBRE_GABINETE") ??
      readString(values, "NOMBRE_GABINETE", "NombreGabinete", "NOMBREGABINETE");

    if (!gabinete) {
      return null;
    }

    const actionResponse = await actionListaDocumentosRadicados(
      buildListaDocumentosRadicadosActionRequest({
        context: { tableId: TABLE_ID, viewMode: "flatDocuments" },
        actionId: "ver_documento",
        rowId,
        nodeType,
        documentId: documentIdFromMeta,
        nombreGabinete: gabinete,
      }),
    );

    const resolveRequest = actionResponse.data?.DocumentResolveRequest;
    if (!actionResponse.success || !resolveRequest) {
      return null;
    }

    const resolved = await resolveDocumentoVisualizacion(resolveRequest);
    const fileUrl = resolveFileUrlFromResolveResponse(resolved);
    if (!fileUrl) {
      return null;
    }

    return {
      fileUrl,
      documentId: resolveRequest.IdDocumento,
      nombreGabinete: resolveRequest.NombreGabinete,
      rowId,
    };
  }, []);

  const onSelectRow = useCallback(async (rowId: string) => performVerDocumento(rowId), [performVerDocumento]);

  const onActionTriggered = useCallback(
    async (params: { actionId: string; rowId: string }) => {
      if (params.actionId === "ver_documento") {
        return performVerDocumento(params.rowId);
      }
      return null;
    },
    [performVerDocumento],
  );

  const getTableColumns = useCallback(() => tableColumns, [tableColumns]);

  return useMemo(
    () => ({
      load,
      loadChildren,
      onSelectRow,
      onActionTriggered,
      getTableColumns,
    }),
    [getTableColumns, load, loadChildren, onActionTriggered, onSelectRow],
  );
};
