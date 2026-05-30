# Build context esperado: a pasta ./frontend (ver docker-compose.yml).
# Stage 1: compila o SPA com Vite. Stage 2: serve os estáticos via nginx,
# que também faz proxy reverso de /api -> container da API (sem CORS).

FROM node:20-alpine AS build
WORKDIR /app

COPY package*.json ./
RUN npm ci

COPY . .
# No container o frontend e a API ficam atrás do mesmo host (nginx),
# então a base da API é relativa. Sobrescreve o valor do .env de dev.
ENV VITE_API_BASE_URL=/api
RUN npm run build

FROM nginx:alpine AS final
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist /usr/share/nginx/html

EXPOSE 80
