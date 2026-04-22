import { useEffect, useRef, useState } from "react";
import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { AppEditor } from "./presentation/AppEditor";
import { AppEditorSaveAction } from "./presentation/AppEditorSaveAction";
import { normalizeEditorHtml } from "./application/normalizeEditorHtml";
import { useAppEditorSaveState } from "./application/useAppEditorSaveState";

function SaveHarness({
  initialValue = "<p></p>",
}: {
  initialValue?: string;
}) {
  const [currentValue, setCurrentValue] = useState(initialValue);
  const [savedValue, setSavedValue] = useState(initialValue);
  const changeCountRef = useRef(0);
  const { saveStatus, isDirty } = useAppEditorSaveState({
    currentValue,
    savedValue,
  });

  useEffect(() => {
    setCurrentValue(initialValue);
    setSavedValue(initialValue);
    changeCountRef.current = 0;
  }, [initialValue]);

  return (
    <div>
      <AppEditor
        title="Editor con guardado"
        label="Contenido"
        value={currentValue}
        onChange={setCurrentValue}
        headerActions={
          <AppEditorSaveAction
            saveStatus={saveStatus}
            onSave={() => {
              setSavedValue(currentValue);
            }}
          />
        }
      />
      <button
        type="button"
        onClick={() => {
          changeCountRef.current += 1;
          setCurrentValue(`<p>Cambio pendiente ${changeCountRef.current}</p>`);
        }}
      >
        Cambiar contenido
      </button>
      <button
        type="button"
        onClick={() => {
          setCurrentValue("<p>Cargado externo</p>");
          setSavedValue("<p>Cargado externo</p>");
        }}
      >
        Cargar externo
      </button>
      <output data-testid="save-status">{saveStatus}</output>
      <output data-testid="dirty-flag">{String(isDirty)}</output>
      <output data-testid="saved-value">{savedValue}</output>
    </div>
  );
}

describe("AppEditorSaveAction [SPEC:IMPLEMENTACION-BOTON-GUARDAR-APPEDITOR-17-FE]", () => {
  it("normaliza representaciones equivalentes de html vacio", () => {
    expect(normalizeEditorHtml("")).toBe("");
    expect(normalizeEditorHtml("<p></p>")).toBe("");
    expect(normalizeEditorHtml("<p><br></p>")).toBe("");
    expect(normalizeEditorHtml("  <p><br /></p>  ")).toBe("");
  });

  it("omite pageBreaks automaticos al normalizar HTML persistible", () => {
    expect(
      normalizeEditorHtml(
        '<p>Uno</p><div data-page-break="true" data-page-break-auto="true" data-page-break-merge="true" data-page-break-spacer="120"></div><p>Dos</p>',
      ),
    ).toBe("<p>Uno</p><p>Dos</p>");
  });

  it("inicia en gris cuando currentValue y savedValue son equivalentes", async () => {
    render(<SaveHarness initialValue="<p></p>" />);

    await waitFor(() => {
      expect(screen.getByTestId("save-status")).toHaveTextContent("idle");
      expect(screen.getByTestId("dirty-flag")).toHaveTextContent("false");
    });

    expect(screen.getByRole("button", { name: "Guardar" })).toBeInTheDocument();
  });

  it("pasa a dirty al editar, vuelve a idle al guardar y retorna a dirty si se edita otra vez", async () => {
    render(<SaveHarness initialValue="<p>Inicio</p>" />);

    fireEvent.click(screen.getByRole("button", { name: "Cambiar contenido" }));

    await waitFor(() => {
      expect(screen.getByTestId("save-status")).toHaveTextContent("dirty");
      expect(screen.getByTestId("dirty-flag")).toHaveTextContent("true");
    });

    fireEvent.click(screen.getByRole("button", { name: "Guardar" }));

    await waitFor(() => {
      expect(screen.getByTestId("save-status")).toHaveTextContent("idle");
      expect(screen.getByTestId("saved-value")).toHaveTextContent("<p>Cambio pendiente 1</p>");
    });

    fireEvent.click(screen.getByRole("button", { name: "Cambiar contenido" }));

    await waitFor(() => {
      expect(screen.getByTestId("save-status")).toHaveTextContent("dirty");
    });
  });

  it("resetea dirty cuando llega un cambio externo que redefine el baseline", async () => {
    render(<SaveHarness initialValue="<p>Inicio</p>" />);

    fireEvent.click(screen.getByRole("button", { name: "Cambiar contenido" }));

    await waitFor(() => {
      expect(screen.getByTestId("save-status")).toHaveTextContent("dirty");
    });

    fireEvent.click(screen.getByRole("button", { name: "Cargar externo" }));

    await waitFor(() => {
      expect(screen.getByTestId("save-status")).toHaveTextContent("idle");
      expect(screen.getByTestId("saved-value")).toHaveTextContent("<p>Cargado externo</p>");
    });
  });
});
