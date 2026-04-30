import { BASE_URL } from '@/shared/api/axiosInstance';

/**
 * Returns a full URL for a photo path.
 * Handles both local paths and absolute Cloudflare R2 URLs.
 */
export const getPhotoUrl = (path: string | null | undefined): string | null => {
  if (!path || path.trim() === '') return null;
  
  // If it's already an absolute URL (like Cloudflare R2), return as is
  if (path.startsWith('http')) return path;
  
  // Otherwise, prepend the base API URL (for local storage)
  const baseUrl = BASE_URL.endsWith('/') ? BASE_URL.slice(0, -1) : BASE_URL;
  const sanitizedPath = path.startsWith('/') ? path : `/${path}`;
  
  return `${baseUrl}${sanitizedPath}`;
};
