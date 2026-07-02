import { act, renderHook, waitFor } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import type { AppUploadDocumentalProps } from "../AppUploadDocumental.types";
import { useAppUploadDocumentalState } from "./useAppUploadDocumentalState";

const props = (overrides: Partial<AppUploadDocumentalProps> = {}): AppUploadDocumentalProps => ({
  proceso: "radicacion",
  context: { nombreGabinete: "Gestion" },
  loadConfig: vi.fn().mockResolvedValue({
    accept: ".pdf",
    allowedExtensions: [".pdf"],
    maxSizeBytes: 100,
    multiple: true,
    requiereTipologia: true,
    requiereFechaCarga: false,
    validationMode: "queue-with-error",
  }),
  loadTiposDocumentales: vi.fn().mockResolvedValue([
    { idTipoDocumento: 1, nombreTipoDocumento: "Contrato arrendamiento" },
  ]),
  ...overrides,
});

describe("[SPEC:SCRUMCORE-271] useAppUploadDocumentalState", () => {
  it("carga loaders, crea metadata independiente y no sobreescribe tipologia manual", async () => {
    const stableProps = props();
    const { result } = renderHook(() => useAppUploadDocumentalState(stableProps));

    await waitFor(() => expect(result.current.config).toBeDefined());

    act(() => {
      result.current.handleFilesSelected([
        new File(["a"], "contrato_arrendamiento.pdf", { type: "application/pdf" }),
        new File(["b"], "otro.pdf", { type: "application/pdf" }),
      ]);
    });

    expect(result.current.files).toHaveLength(2);
    expect(result.current.files[0].metadata?.idTipoDocumento).toBe(1);

    const secondUid = result.current.files[1].uid;
    act(() => {
      result.current.updateMetadata(secondUid, { idTipoDocumento: 99, nombreTipoDocumento: "Manual" }, true);
    });

    expect(result.current.files[1].metadata).toMatchObject({
      idTipoDocumento: 99,
      nombreTipoDocumento: "Manual",
      tipologiaManual: true,
    });
  });

  it("delega mensaje de tipologia al backend y valida fecha obligatoria por archivo", async () => {
    const stableProps = props({
      tipologiaObligatoria: true,
      requiereFechaCarga: true,
      fechaCargaObligatoria: true,
    });
    const { result } = renderHook(() => useAppUploadDocumentalState(stableProps));

    await waitFor(() => expect(result.current.config).toBeDefined());

    act(() => {
      result.current.handleFilesSelected([new File(["a"], "otro.pdf", { type: "application/pdf" })]);
    });

    const uid = result.current.files[0].uid;
    expect(result.current.validateFileForStore(uid)).toBe(
      "No se puede guardar: ingresa la fecha documental del archivo.",
    );

    act(() => {
      result.current.updateMetadata(uid, { idTipoDocumento: 1, nombreTipoDocumento: "Contrato" }, true);
    });
    expect(result.current.validateFileForStore(uid)).toBe(
      "No se puede guardar: ingresa la fecha documental del archivo.",
    );

    act(() => {
      result.current.updateMetadata(uid, { fechaCarga: "2099-01-01" });
    });
    expect(result.current.validateFileForStore(uid)).toBe(
      "No se puede guardar: la fecha documental debe ser real, no futura y usar formato AAAA-MM-DD.",
    );
  });
});
