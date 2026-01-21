import axios from "axios";
import type { 
  ProjectResponseDto, 
  ProjectUpsertDto 
} from "../types/project.types";

const http = axios.create({
  baseURL: import.meta.env.VITE_API_URL || "http://localhost:5000/api",
  headers: {
    'Content-Type': 'application/json'
  }
});

export const ProjectsApi = {
  async getAll(): Promise<ProjectResponseDto[]> {
    const { data } = await http.get<ProjectResponseDto[]>("/projects");
    return data;
  },

  async getById(id: string): Promise<ProjectResponseDto> {
    const { data } = await http.get<ProjectResponseDto>(`/projects/${id}`);
    return data;
  },

  async create(dto: ProjectUpsertDto): Promise<ProjectResponseDto> {
    // Conver Date for API
    const payload = {
      ...dto,
      params: {
        ...dto.params,
        startDate: dto.params.startDate.includes('T') 
          ? dto.params.startDate 
          : `${dto.params.startDate}T00:00:00`,
        endDate: dto.params.endDate.includes('T') 
          ? dto.params.endDate 
          : `${dto.params.endDate}T00:00:00`,
      }
    };
    
    const { data } = await http.post<ProjectResponseDto>("/projects", payload);
    return data;
  },

  async update(id: string, dto: ProjectUpsertDto): Promise<void> {
    const payload = {
      ...dto,
      params: {
        ...dto.params,
        startDate: dto.params.startDate.includes('T') 
          ? dto.params.startDate 
          : `${dto.params.startDate}T00:00:00`,
        endDate: dto.params.endDate.includes('T') 
          ? dto.params.endDate 
          : `${dto.params.endDate}T00:00:00`,
      }
    };
    
    await http.put(`/projects/${id}`, payload);
  },

  async delete(id: string): Promise<void> {
    await http.delete(`/projects/${id}`);
  }
};