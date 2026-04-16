# AppCheckbox

Familia shared de checkboxes construida sobre Ant Design.

## Componentes

- `AppCheckbox`
- `AppCheckboxGroup`
- `AppCheckboxCheckAll`

## Ejemplo simple

```tsx
<AppCheckbox
  label="Acepto terminos y condiciones"
  onChange={(checked) => console.log(checked)}
/>
```

## Ejemplo de grupo controlado

```tsx
const [channels, setChannels] = useState<string[]>(["correo"]);

<AppCheckboxGroup
  label="Canales de notificacion"
  onChange={setChannels}
  options={[
    { label: "Correo", value: "correo" },
    { label: "SMS", value: "sms" },
  ]}
  value={channels}
/>
```

## Ejemplo de check all

```tsx
const [channels, setChannels] = useState<string[]>([]);

<AppCheckboxCheckAll
  checkAllLabel="Seleccionar todos"
  onChange={setChannels}
  options={[
    { label: "Correo", value: "correo" },
    { label: "SMS", value: "sms" },
    { label: "WhatsApp", value: "whatsapp", disabled: true },
  ]}
  value={channels}
/>
```

## Integracion con Form.Item

```tsx
<Form.Item
  label="Permisos"
  name="permisos"
  rules={[{ required: true, message: "Selecciona al menos una opcion" }]}
>
  <AppCheckboxGroup
    onChange={setPermisos}
    options={[
      { label: "Lectura", value: "read" },
      { label: "Escritura", value: "write" },
    ]}
    value={permisos}
  />
</Form.Item>
```

## Notas

- `AppCheckboxGroup` y `AppCheckboxCheckAll` son controlados.
- `AppCheckboxCheckAll` selecciona y limpia solo las opciones habilitadas.
- El estado parcial se refleja mediante `indeterminate`.
