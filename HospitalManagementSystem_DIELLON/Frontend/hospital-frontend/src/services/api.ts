import type { Patient, Doctor, Appointment, Department, Invoice, LabResult, Prescription, AuthResponse, MedicalRecord, User } from '../types'

const API_BASE_URL = 'http://localhost:5143/api'

// Helper function for authenticated requests
const getHeaders = () => {
  const token = localStorage.getItem('token')
  return {
    'Content-Type': 'application/json',
    ...(token ? { Authorization: `Bearer ${token}` } : {}),
  }
}

// Generic API request handler
async function apiRequest<T>(
  endpoint: string,
  options: RequestInit = {}
): Promise<T> {
  const response = await fetch(`${API_BASE_URL}${endpoint}`, {
    ...options,
    headers: {
      ...getHeaders(),
      ...options.headers,
    },
  })

  if (!response.ok) {
    const errorData = await response.text()
    throw new Error(errorData || `HTTP error! status: ${response.status}`)
  }

  // Handle empty responses
  const text = await response.text()
  if (!text) return {} as T
  
  return JSON.parse(text)
}

// Auth API
export const authApi = {
  login: async (username: string, password: string): Promise<AuthResponse> => {
    const response = await apiRequest<{ token: string; user: { username: string; role: string } }>(
      '/User/login',
      {
        method: 'POST',
        body: JSON.stringify({ username, password }),
      }
    )
    return {
      token: response.token,
      username: response.user.username,
      role: response.user.role,
      expiresAt: new Date(Date.now() + 24 * 60 * 60 * 1000).toISOString(),
    }
  },

  register: async (username: string, email: string, password: string, role: string): Promise<AuthResponse> => {
    const response = await apiRequest<{ token: string; user: { username: string; role: string } }>(
      '/User/register',
      {
        method: 'POST',
        body: JSON.stringify({ username, email, password, role }),
      }
    )
    return {
      token: response.token || '',
      username: response.user?.username || username,
      role: response.user?.role || role,
      expiresAt: new Date(Date.now() + 24 * 60 * 60 * 1000).toISOString(),
    }
  },
}

// Users API
export const usersApi = {
  getAll: async (): Promise<User[]> => {
    return apiRequest<User[]>('/User')
  },

  getById: async (id: number): Promise<User> => {
    return apiRequest<User>(`/User/${id}`)
  },
}

// Patients API
export const patientsApi = {
  getAll: async (): Promise<Patient[]> => {
    return apiRequest<Patient[]>('/Patient')
  },

  getById: async (id: number): Promise<Patient> => {
    return apiRequest<Patient>(`/Patient/${id}`)
  },

  create: async (patient: Omit<Patient, 'patientId'>): Promise<Patient> => {
    return apiRequest<Patient>('/Patient', {
      method: 'POST',
      body: JSON.stringify(patient),
    })
  },

  update: async (id: number, patient: Partial<Patient>): Promise<Patient> => {
    return apiRequest<Patient>(`/Patient/${id}`, {
      method: 'PUT',
      body: JSON.stringify(patient),
    })
  },

  delete: async (id: number): Promise<void> => {
    return apiRequest<void>(`/Patient/${id}`, {
      method: 'DELETE',
    })
  },
}

// Doctors API
export const doctorsApi = {
  getAll: async (): Promise<Doctor[]> => {
    return apiRequest<Doctor[]>('/Staff/doctors')
  },

  getById: async (id: number): Promise<Doctor> => {
    return apiRequest<Doctor>(`/Staff/doctors/${id}`)
  },

  create: async (doctor: Omit<Doctor, 'doctorId'>): Promise<Doctor> => {
    return apiRequest<Doctor>('/Staff/doctors', {
      method: 'POST',
      body: JSON.stringify(doctor),
    })
  },

  update: async (id: number, doctor: Partial<Doctor>): Promise<Doctor> => {
    return apiRequest<Doctor>(`/Staff/doctors/${id}`, {
      method: 'PUT',
      body: JSON.stringify(doctor),
    })
  },

  delete: async (id: number): Promise<void> => {
    return apiRequest<void>(`/Staff/doctors/${id}`, {
      method: 'DELETE',
    })
  },
}

// Appointments API
export const appointmentsApi = {
  getAll: async (): Promise<Appointment[]> => {
    return apiRequest<Appointment[]>('/Appointment')
  },

  getById: async (id: number): Promise<Appointment> => {
    return apiRequest<Appointment>(`/Appointment/${id}`)
  },

  getByPatient: async (patientId: number): Promise<Appointment[]> => {
    return apiRequest<Appointment[]>(`/Appointment/patient/${patientId}`)
  },

  getByDoctor: async (doctorId: number): Promise<Appointment[]> => {
    return apiRequest<Appointment[]>(`/Appointment/doctor/${doctorId}`)
  },

  create: async (appointment: {
    patientId: number
    doctorId: number
    time: string
    reason: string
  }): Promise<Appointment> => {
    return apiRequest<Appointment>('/Appointment', {
      method: 'POST',
      body: JSON.stringify(appointment),
    })
  },

  update: async (id: number, appointment: Partial<Appointment>): Promise<Appointment> => {
    return apiRequest<Appointment>(`/Appointment/${id}`, {
      method: 'PUT',
      body: JSON.stringify(appointment),
    })
  },

  cancel: async (id: number): Promise<void> => {
    return apiRequest<void>(`/Appointment/${id}`, {
      method: 'DELETE',
    })
  },

  checkAvailability: async (doctorId: number, appointmentTime: string): Promise<boolean> => {
    return apiRequest<boolean>(`/Appointment/check-availability?doctorId=${doctorId}&appointmentTime=${appointmentTime}`)
  },
}

// Departments API
export const departmentsApi = {
  getAll: async (): Promise<Department[]> => {
    return apiRequest<Department[]>('/Department')
  },

  getById: async (id: number): Promise<Department> => {
    return apiRequest<Department>(`/Department/${id}`)
  },

  create: async (department: Omit<Department, 'departmentId'>): Promise<Department> => {
    return apiRequest<Department>('/Department', {
      method: 'POST',
      body: JSON.stringify(department),
    })
  },

  update: async (id: number, department: Partial<Department>): Promise<Department> => {
    return apiRequest<Department>(`/Department/${id}`, {
      method: 'PUT',
      body: JSON.stringify(department),
    })
  },

  delete: async (id: number): Promise<void> => {
    return apiRequest<void>(`/Department/${id}`, {
      method: 'DELETE',
    })
  },
}

// Invoices API
export const invoicesApi = {
  getAll: async (): Promise<Invoice[]> => {
    return apiRequest<Invoice[]>('/Invoice')
  },

  getById: async (id: number): Promise<Invoice> => {
    return apiRequest<Invoice>(`/Invoice/${id}`)
  },

  getByPatient: async (patientId: number): Promise<Invoice[]> => {
    return apiRequest<Invoice[]>(`/Invoice/patient/${patientId}`)
  },

  create: async (invoice: { patientId: number; total: number }): Promise<Invoice> => {
    return apiRequest<Invoice>('/Invoice', {
      method: 'POST',
      body: JSON.stringify(invoice),
    })
  },

  updateStatus: async (id: number, status: string): Promise<Invoice> => {
    return apiRequest<Invoice>(`/Invoice/${id}`, {
      method: 'PUT',
      body: JSON.stringify({ status }),
    })
  },

  processPayment: async (invoiceId: number, amount: number, paymentMethod: string): Promise<void> => {
    return apiRequest<void>('/Invoice/payment', {
      method: 'POST',
      body: JSON.stringify({ invoiceId, amount, paymentMethod }),
    })
  },
}

// Lab Results API
export const labResultsApi = {
  getAll: async (): Promise<LabResult[]> => {
    return apiRequest<LabResult[]>('/LabResult')
  },

  getById: async (id: number): Promise<LabResult> => {
    return apiRequest<LabResult>(`/LabResult/${id}`)
  },

  getByPatient: async (patientId: number): Promise<LabResult[]> => {
    return apiRequest<LabResult[]>(`/LabResult/patient/${patientId}`)
  },

  create: async (labResult: Omit<LabResult, 'resultId'>): Promise<LabResult> => {
    return apiRequest<LabResult>('/LabResult', {
      method: 'POST',
      body: JSON.stringify(labResult),
    })
  },

  update: async (id: number, labResult: Partial<LabResult>): Promise<LabResult> => {
    return apiRequest<LabResult>(`/LabResult/${id}`, {
      method: 'PUT',
      body: JSON.stringify(labResult),
    })
  },

  delete: async (id: number): Promise<void> => {
    return apiRequest<void>(`/LabResult/${id}`, {
      method: 'DELETE',
    })
  },
}

// Prescriptions API
export const prescriptionsApi = {
  getAll: async (): Promise<Prescription[]> => {
    return apiRequest<Prescription[]>('/Prescription')
  },

  getById: async (id: number): Promise<Prescription> => {
    return apiRequest<Prescription>(`/Prescription/${id}`)
  },

  getByPatient: async (patientId: number): Promise<Prescription[]> => {
    return apiRequest<Prescription[]>(`/Prescription/patient/${patientId}`)
  },

  create: async (prescription: Omit<Prescription, 'prescriptionId'>): Promise<Prescription> => {
    return apiRequest<Prescription>('/Prescription', {
      method: 'POST',
      body: JSON.stringify(prescription),
    })
  },

  update: async (id: number, prescription: Partial<Prescription>): Promise<Prescription> => {
    return apiRequest<Prescription>(`/Prescription/${id}`, {
      method: 'PUT',
      body: JSON.stringify(prescription),
    })
  },

  delete: async (id: number): Promise<void> => {
    return apiRequest<void>(`/Prescription/${id}`, {
      method: 'DELETE',
    })
  },
}

// Medical Records API
export const medicalRecordsApi = {
  getByPatient: async (patientId: number): Promise<MedicalRecord> => {
    return apiRequest<MedicalRecord>(`/MedicalRecord/patient/${patientId}`)
  },

  getById: async (id: number): Promise<MedicalRecord> => {
    return apiRequest<MedicalRecord>(`/MedicalRecord/${id}`)
  },

  create: async (record: { patientId: number; diagnosis?: string; treatment?: string }): Promise<MedicalRecord> => {
    return apiRequest<MedicalRecord>('/MedicalRecord', {
      method: 'POST',
      body: JSON.stringify(record),
    })
  },

  update: async (id: number, record: { diagnosis?: string; treatment?: string }): Promise<MedicalRecord> => {
    return apiRequest<MedicalRecord>(`/MedicalRecord/${id}`, {
      method: 'PUT',
      body: JSON.stringify(record),
    })
  },

  addClinicalEntry: async (entry: { recordId: number; notes: string; diagnosis?: string }): Promise<ClinicalEntry> => {
    return apiRequest<ClinicalEntry>('/MedicalRecord/clinical-entry', {
      method: 'POST',
      body: JSON.stringify(entry),
    })
  },
}

