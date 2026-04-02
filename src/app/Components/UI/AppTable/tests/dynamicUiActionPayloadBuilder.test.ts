import { describe, expect, it } from "vitest";
import { buildDynamicUiActionPayload } from "../utils/dynamicUiActionPayloadBuilder";

const action = {
  actionId: "gestionar_tramite",
  label: "Gestionar",
  placement: "row",
  presentation: "icon_button",
  behavior: "client_event",
  request: {
    RowIdField: "id_tarea",
    PayloadFields: {
      tramiteId: "id_tarea",
      beneficiarios: "BENEFICIARIO",
    },
    source: "backend",
    tramiteId: "request-value",
  },
  payload: {
    source: "action",
    keep: true,
  },
};

describe("[SPEC:CREA-ACTION-LAYER-AG-GRID] dynamicUiActionPayloadBuilder", () => {
  it("builds payload with derived values, request metadata and manual override precedence", () => {
    const context = {
      row: {
        id: "924",
        data: {
          id_tarea: 924,
          BENEFICIARIO: "Yeraldi Alvarado",
        },
      },
      selectedRows: [
        {
          id: "924",
          data: {
            id_tarea: 924,
            BENEFICIARIO: "Yeraldi Alvarado",
          },
        },
      ],
    };

    const result = buildDynamicUiActionPayload(action, context, {
      source: "manual",
      tramiteId: "manual-value",
    });

    expect(result).toEqual({
      tramiteId: "manual-value",
      beneficiarios: "Yeraldi Alvarado",
      rowId: 924,
      selectedRowIds: ["924"],
      source: "manual",
      keep: true,
    });
  });

  it("derives selected values from multiple rows when there is no single current row", () => {
    const result = buildDynamicUiActionPayload(
      {
        ...action,
        request: {
          PayloadFields: {
            ids: "id_tarea",
          },
        },
      },
      {
        selectedRows: [
          { id: "924", data: { id_tarea: 924 } },
          { id: "923", data: { id_tarea: 923 } },
        ],
      },
    );

    expect(result).toEqual({
      ids: [924, 923],
      selectedRowIds: ["924", "923"],
      source: "action",
      keep: true,
    });
  });

  it("falls back to the current row id when the request does not declare RowIdField", () => {
    const result = buildDynamicUiActionPayload(
      {
        ...action,
        request: {},
      },
      {
        row: {
          id: "924",
          data: {
            id_tarea: 924,
          },
        },
      },
    );

    expect(result).toEqual({
      rowId: "924",
      keep: true,
      source: "action",
    });
  });
});
