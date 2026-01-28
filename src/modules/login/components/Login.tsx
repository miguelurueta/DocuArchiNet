import React, { useState, useEffect } from "react";
import { useEmpresaActual } from "../hooks/useEmpresaActual";
import { useModulosEmpresa } from "../hooks/useModulosEmpresa";
import { useAxiosErrorNotifier } from "../../../shared/hooks/useAxiosErrorNotifier";

import RequiredTooltip from "../../../app/Components/RequiredTooltip";
import useLogin from "../hooks/useLogin";
import type LoginRequestDTO from "../models/LoginRequestDTO.model";
import styles from "../Style/login.module.css";
import { useNavigate } from "react-router";

export default function Login() {
  const notifyAxiosError = useAxiosErrorNotifier();
  const navigate = useNavigate();
  const { login, isLoading } = useLogin();

  // ===========================
  // Estado
  // ===========================
  const [openModulos, setOpenModulos] = useState(false);
  const [idModulo, setIdModulo] = useState<number>(0);
  const [showPassword, setShowPassword] = useState(false);

  const [usuario, setUsuario] = useState("");
  const [password, setPassword] = useState("");
  const [submitIntentado, setSubmitIntentado] = useState(false);

  // 🆕 validación puntual por campo
  const [invalidField, setInvalidField] = useState<string | null>(null);

  // ===========================
  // Empresa + módulos
  // ===========================
  const {
    empresa,
    isLoading: isLoadingEmpresa,
    isError: isErrorEmpresa,
    error: errorEmpresa,
  } = useEmpresaActual();

  const idEmpresa = empresa?.IdEmpresa ?? 0;

  const {
    data: modulosResponse,
    isLoading: isLoadingModulos,
    isError: isErrorModulos,
    error: errorModulos,
  } = useModulosEmpresa(idEmpresa);

  const modulos = modulosResponse?.data ?? [];

  // ===========================
  // Manejo de errores
  // ===========================
  useEffect(() => {
    if (isErrorEmpresa) notifyAxiosError(errorEmpresa);
  }, [isErrorEmpresa, errorEmpresa]);

  useEffect(() => {
    if (isErrorModulos) notifyAxiosError(errorModulos);
  }, [isErrorModulos, errorModulos]);

  // ===========================
  // Click fuera
  // ===========================
  useEffect(() => {
    const handleClickOutside = (e: MouseEvent) => {
      const select = document.getElementById("selectModulos");
      if (select && !select.contains(e.target as Node)) {
        setOpenModulos(false);
      }
    };
    document.addEventListener("mousedown", handleClickOutside);
    return () =>
      document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  // ===========================
  // Submit login (legacy)
  // ===========================
  const onSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitIntentado(true);
    // 🔗 reconecta submit con validación por campo
  if (idModulo === 0) {
    setInvalidField("IdModulo");
  }
    const loginData: LoginRequestDTO = {
      User: usuario,
      Password: password,
      IdModulo: idModulo,
      IdEmpresa: idEmpresa,
    };

    if (usuario && password && idModulo !== 0) {
      setInvalidField(null); // limpia errores puntuales
      login(loginData);
    }
  };

  // ===========================
  // Navegación recovery (nuevo)
  // ===========================
  
  const OnNavigator = (e: React.MouseEvent<HTMLAnchorElement>) => {
    e.preventDefault();

    if (idModulo === 0) {
      setInvalidField("IdModulo");
      return;
    }
    
    navigate("/recovery-password/forgot-password", {
      state: {
        idModulo,
        idEmpresa,
        loginUsuario: usuario
      },
    });
  };

  // ===========================
  // JSX
  // ===========================
  return (
    <div className={styles.contenedor_}>
      <div>
        <form id="login_usuario" onSubmit={onSubmit}>
          <h2>
            <img src="/contasoft.png" width="230" height="120" />
          </h2>

          {/* ===========================
              Módulos
          =========================== */}
          <div
            id="selectModulos"
            className={`${styles["custom-select"]} ${
              openModulos ? styles.open : ""
            }`}
          >
            <div
              className={styles.selected}
              onClick={() => setOpenModulos((o) => !o)}
            >
              <span>
                {modulos.find((m) => m.IdModulo === idModulo)?.NombreModulo ??
                  ""}
              </span>
              <i className="fa-solid fa-circle-chevron-down"></i>
            </div>

            <ul className={styles.options}>
              {modulos.map((m) => (
                <li
                  key={m.IdModulo}
                  onClick={() => {
                    setIdModulo(m.IdModulo);
                    setInvalidField(null); // limpia error puntual
                    setOpenModulos(false);
                  }}
                >
                  {m.NombreModulo}
                </li>
              ))}
            </ul>

            <RequiredTooltip
              field="IdModulo"
              invalidField={invalidField}
              message="Debe seleccionar un módulo"
            />
          </div>

          {/* ===========================
              Usuario
          =========================== */}
          <div className={styles["input-contenedor"]}>
            <i className="fa-solid fa-user"></i>
            <input
              id="usuario"
              type="text"
              placeholder=" "
              value={usuario}
              onChange={(e) => setUsuario(e.target.value)}
            />
            <label htmlFor="usuario">usuario</label>
            <RequiredTooltip visible={submitIntentado && !usuario} message="Debe informar el usuario" />
          </div>

          {/* ===========================
              Password
          =========================== */}
          <div className={styles["input-contenedor"]}>
            <i className="fa-solid fa-lock"></i>
            <input
              type={showPassword ? "text" : "contraseña"}
              id="password"
              placeholder=" "
              value={password}
              onChange={(e) => setPassword(e.target.value)}
            />
            <label htmlFor="password">Contraseña</label>
            <RequiredTooltip visible={submitIntentado && !password} message="Debe informar la contraseña" />
          </div>

          {/* ===========================
              Toggle
          =========================== */}
          <div className={styles["switch-contenedor"]}>
            <label className={styles.switch}>
              <input
                type="checkbox"
                onChange={() => setShowPassword((v) => !v)}
              />
              <span className={styles.slider}></span>
            </label>
          </div>

          {/* ===========================
              Botón
          =========================== */}
          <div className={styles.aaa}>
            <button
              type="submit"
              disabled={isLoadingEmpresa || isLoadingModulos}
            >
              Iniciar Sesión
            </button>
          </div>

          {/* ===========================
              Olvidé contraseña
          =========================== */}
          <a className={styles.olc} onClick={OnNavigator} href="#">
            <i className="fa-solid fa-key"></i>
            <span>¿Olvidaste la contraseña?</span>
          </a>
        </form>
      </div>
    </div>
  );
}
