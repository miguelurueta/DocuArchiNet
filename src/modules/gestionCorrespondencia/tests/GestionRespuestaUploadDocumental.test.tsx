import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { GestionRespuestaUploadDocumental } from "../components/gestionRespuestaMainTab/GestionRespuestaUploadDocumental";

const refreshDocumentosSpy = vi.fn();
const appUploadDocumentalSpy = vi.fn();

vi.mock("../hooks/useGestionRespuestaDocumentos", () => ({
  useGestionRespuestaDocumentos: () => ({
    idTareaWf: 933,
    radicado: "2600466700021",
    idRespuestaRadicado: 672,
    nombreGabinete: "CORRESPO",
    gabineteLoading: false,
    gabineteError: undefined,
    refreshDocumentos: refreshDocumentosSpy,
  }),
}));

vi.mock("../../almacenamientoDocumental/components/AppUploadDocumental", () => ({
  AppUploadDocumental: (props: {
    title?: string;
    context: unknown;
    storageOptions?: unknown;
    buildStoreRequest?: unknown;
    onStored?: (result: { rawBackendResult?: unknown }) => void;
  }) => {
    appUploadDocumentalSpy(props);
    return (
      <div data-testid="app-upload-documental-mock">
        <span>{props.title}</span>
        <button
          type="button"
          onClick={() =>
            props.onStored?.({
              rawBackendResult: {
                Documento: {
                  IdAlmacen: 9967,
                  IdRegistroProduccionDocumental: 23040,
                  NombreArchivoFinal: "DIG00009967.pdf",
                },
                AnexoRespuesta: {
                  IdRespuestaRadicado: 672,
                  IdAlmacen: 9967,
                  NombreGabinete: "CORRESPO",
                  NombreArchivo: "soporte-respuesta.pdf",
                  Created: true,
                },
              },
            })
          }
        >
          Simular stored
        </button>
      </div>
    );
  },
}));

describe("[SCRUMCORE-277] GestionRespuestaUploadDocumental", () => {
  it("renderiza AppUploadDocumental con contexto, mapper y opciones enterprise", () => {
    render(<GestionRespuestaUploadDocumental />);

    expect(screen.getByTestId("app-upload-documental-mock")).toBeInTheDocument();
    expect(appUploadDocumentalSpy).toHaveBeenCalledWith(
      expect.objectContaining({
        title: "Adjuntos",
        tipologiaObligatoria: true,
        buildStoreRequest: expect.any(Function),
        storageOptions: {
          backendPayloadCase: "pascal",
          validateStatusBeforeComplete: true,
        },
        context: expect.objectContaining({
          nombreGabinete: "CORRESPO",
          idTareaWorkflow: 933,
          idRespuesta: 672,
          nameModulo: "2600466700021",
        }),
      }),
    );
  });

  it("refresca documentos cuando el backend confirma AnexoRespuesta.Created", () => {
    render(<GestionRespuestaUploadDocumental />);

    screen.getByRole("button", { name: "Simular stored" }).click();

    expect(refreshDocumentosSpy).toHaveBeenCalledTimes(1);
  });
});
