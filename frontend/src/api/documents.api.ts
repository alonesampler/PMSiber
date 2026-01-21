import axios from "axios";
import type { DocumentResponseDto } from "../types/project.types";

const http = axios.create({
  baseURL: import.meta.env.VITE_API_URL || "http://localhost:5000/api",
});

export const DocumentsApi = {
  async getByProject(projectId: string): Promise<DocumentResponseDto[]> {
    const { data } = await http.get<DocumentResponseDto[]>(`/documents/project/${projectId}`);
    return data;
  },

  async upload(projectId: string, file: File): Promise<DocumentResponseDto> {
    const formData = new FormData();
    
    formData.append("file", file);
    formData.append("ProjectId", projectId);
    formData.append("FileName", file.name);
    formData.append("ContentType", file.type || "application/octet-stream");
    formData.append("FileSize", file.size.toString());

    const { data } = await http.post<DocumentResponseDto>(
      "/documents/upload", 
      formData,
      {
        headers: { 
          "Content-Type": "multipart/form-data" 
        }
      }
    );
    return data;
  },

  async delete(id: string): Promise<void> {
    await http.delete(`/documents/${id}`);
  },

  async download(id: string): Promise<Blob> {
    const response = await http.get(`/documents/${id}/download`, {
      responseType: 'blob'
    });
    return response.data;
  }
};