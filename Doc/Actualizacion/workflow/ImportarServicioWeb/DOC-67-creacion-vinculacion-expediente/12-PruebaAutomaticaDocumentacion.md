# Prueba automática de documentación

## Implementación

`tests/importar-servicio-web-doc67-documentation.test.cjs` reutiliza `node:test` y entra automáticamente en `Scripts/run-doc67-readiness.cjs` mediante el patrón `tests/importar-servicio-web-*.test.cjs`.

El manifiesto explícito `Diagramas/manifest.json` enumera los cinco diagramas obligatorios. La prueba falla indicando archivo, símbolo y motivo cuando:

- falta el manifiesto o un diagrama requerido;
- falta una fuente de código declarada;
- el bloque Mermaid no pertenece al subconjunto soportado o tiene bloques de secuencia desbalanceados;
- un símbolo `CODE:` no está declarado en `Referencias CODE:`;
- una clase o interfaz VB.NET no existe;
- un método no pertenece al tipo indicado;
- ninguna sobrecarga coincide exactamente en parámetros y retorno.

La resolución es estructural: el analizador construye tipos y métodos por ámbito desde declaraciones VB.NET, une firmas multilínea y compara propietario, sobrecarga, tipos de parámetros y retorno. `EXT:` y `CONCEPT:` son exclusiones explícitas; no se resuelven contra código.

## Ejecución

```powershell
node --test tests/importar-servicio-web-doc67-documentation.test.cjs
node Scripts/run-doc67-readiness.cjs
```

Resultado del 17 de septiembre de 2026: prueba focal 4/4; readiness integral 336/336 y compilación VB.NET satisfactoria (`READINESS_OK`).

## Límite probatorio

El validador Mermaid comprueba la gramática usada por este paquete y el balance de bloques de secuencia; no incorpora el renderizador oficial de Mermaid. La prueba demuestra existencia y correspondencia estructural, pero por sí sola no demuestra fidelidad conductual completa, corrección del proveedor externo ni cobertura de todos los casos de uso. La revisión humana y las pruebas de contrato, integración y E2E siguen siendo necesarias.
