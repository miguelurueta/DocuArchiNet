import { useEffect, useState } from "react";
import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { useAppEditor } from "./application/useAppEditor";

type HarnessProps = {
  value?: string;
  defaultValue?: string;
  placeholder?: string;
  disabled?: boolean;
  readOnly?: boolean;
  onChange?: (value: string) => void;
};

function HookHarness(props: HarnessProps) {
  const { editor, isEditable } = useAppEditor(props);
  const [html, setHtml] = useState("");

  useEffect(() => {
    if (!editor) {
      return;
    }

    const syncSnapshot = () => {
      setHtml(editor.getHTML());
    };

    syncSnapshot();
    editor.on("update", syncSnapshot);
    editor.on("transaction", syncSnapshot);

    return () => {
      editor.off("update", syncSnapshot);
      editor.off("transaction", syncSnapshot);
    };
  }, [editor]);

  return (
    <div>
      <button
        type="button"
        onClick={() => editor?.commands.setContent("<p>Nuevo contenido</p>")}
      >
        set-content
      </button>
      <button
        type="button"
        onClick={() => editor?.commands.undo()}
      >
        undo
      </button>
      <output data-testid="editable">{String(isEditable)}</output>
      <output data-testid="html">{html}</output>
    </div>
  );
}

describe("useAppEditor [SPEC:IMPLEMENTACION-COMPONENTE-APPEDITOR-01-FE]", () => {
  it("inicializa en modo no controlado y propaga onChange", async () => {
    const handleChange = vi.fn();

    render(
      <HookHarness
        defaultValue="<p>Inicial</p>"
        onChange={handleChange}
      />,
    );

    await waitFor(() => {
      expect(screen.getByTestId("html")).toHaveTextContent("Inicial");
    });

    fireEvent.click(screen.getByText("set-content"));

    await waitFor(() => {
      expect(handleChange).toHaveBeenCalledWith(expect.stringContaining("Nuevo contenido"));
      expect(screen.getByTestId("html")).toHaveTextContent("Nuevo contenido");
    });
  });

  it("sincroniza el valor controlado externamente", async () => {
    const { rerender } = render(<HookHarness value="<p>Uno</p>" />);

    await waitFor(() => {
      expect(screen.getByTestId("html")).toHaveTextContent("Uno");
    });

    rerender(<HookHarness value="<p>Dos</p>" />);

    await waitFor(() => {
      expect(screen.getByTestId("html")).toHaveTextContent("Dos");
    });
  });

  it("desactiva la edicion cuando disabled o readOnly estan activos", () => {
    const { rerender } = render(<HookHarness disabled />);

    expect(screen.getByTestId("editable")).toHaveTextContent("false");

    rerender(<HookHarness readOnly />);

    expect(screen.getByTestId("editable")).toHaveTextContent("false");
  });
});
