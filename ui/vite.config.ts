import { defineConfig, loadEnv } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), '')
  const extraHosts = (env.ALLOWED_HOST || env.VITE_ALLOWED_HOST || '')
    .split(',')
    .map((h: string) => h.trim())
    .filter(Boolean)

  return {
    plugins: [react({
      babel: {
        plugins: [["babel-plugin-react-compiler"]]
      }
    })],
    server: {
      allowedHosts: [
        'localhost',
        '127.0.0.1',
        '0.0.0.0',
        ...extraHosts,
      ],
    },
  }
})