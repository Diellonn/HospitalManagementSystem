import { useEffect, useState } from 'react'
import { invoicesApi, patientsApi } from '../services/api'
import { useAuth } from '../context/AuthContext'
import type { Invoice, Patient } from '../types'
import './Common.css'

export default function Invoices() {
  const { user } = useAuth()
  const [invoices, setInvoices] = useState<Invoice[]>([])
  const [patients, setPatients] = useState<Patient[]>([])
  const [loading, setLoading] = useState(true)
  const [showModal, setShowModal] = useState(false)
  const [formData, setFormData] = useState({
    patientId: 0,
    total: 0,
  })

  useEffect(() => {
    loadData()
  }, [])

  const loadData = async () => {
    try {
      const isPatient = user?.role === 'Patient'
      
      if (isPatient) {
        // For patients, find their patient record and load only their invoices
        const patientsData = await patientsApi.getAll()
        const foundPatient = patientsData.find(p => p.email === user?.email || p.name === user?.username)
        
        if (foundPatient) {
          const invoicesData = await invoicesApi.getByPatient(foundPatient.patientId!)
          setInvoices(invoicesData)
        }
      } else {
        // For admins, load all invoices
        const [invoicesData, patientsData] = await Promise.all([
          invoicesApi.getAll(),
          patientsApi.getAll()
        ])
        setInvoices(invoicesData)
        setPatients(patientsData)
      }
    } catch (error) {
      console.error('Error loading data:', error)
    } finally {
      setLoading(false)
    }
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    
    if (formData.patientId === 0) {
      alert('Ju lutem zgjidhni një pacient')
      return
    }
    
    if (formData.total <= 0) {
      alert('Totali duhet të jetë më i madh se 0')
      return
    }
    
    try {
      await invoicesApi.create({ patientId: formData.patientId, total: formData.total })
      setShowModal(false)
      setFormData({ patientId: 0, total: 0 })
      loadData()
    } catch (error: any) {
      console.error('Error creating invoice:', error)
      const errorMessage = error?.message || 'Gabim në krijimin e faturës'
      alert(errorMessage)
    }
  }

  const handleStatusUpdate = async (id: number, status: string) => {
    try {
      await invoicesApi.updateStatus(id, status)
      loadData()
    } catch (error) {
      console.error('Error updating invoice status:', error)
    }
  }

  if (loading) return <div>Loading...</div>

  const isPatient = user?.role === 'Patient'

  return (
    <div className="page-container">
      <div className="page-header">
        <h1>Faturat</h1>
        {!isPatient && (
          <button onClick={() => { setShowModal(true); setFormData({ patientId: 0, total: 0 }) }} className="btn-primary">
            + Shto Faturë
          </button>
        )}
      </div>

      <table className="data-table">
        <thead>
          <tr>
            <th>ID</th>
            <th>Pacienti</th>
            <th>Totali</th>
            <th>Statusi</th>
            <th>Data e Lëshimit</th>
            <th>Data e Pagesës</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {invoices.map((invoice) => (
            <tr key={invoice.invoiceId}>
              <td>{invoice.invoiceId}</td>
              <td>{invoice.patientName || invoice.patient?.name || `Patient ${invoice.patientId}`}</td>
              <td>{invoice.total.toFixed(2)} ALL</td>
              <td><span className={`status-badge status-${invoice.status.toLowerCase()}`}>{invoice.status}</span></td>
              <td>{invoice.dateIssued ? new Date(invoice.dateIssued).toLocaleDateString() : '-'}</td>
              <td>{invoice.datePaid ? new Date(invoice.datePaid).toLocaleDateString() : '-'}</td>
              <td>
                {invoice.status === 'Pending' && !isPatient && (
                  <button onClick={() => handleStatusUpdate(invoice.invoiceId!, 'Paid')} className="btn-edit">Mark as Paid</button>
                )}
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      {showModal && (
        <div className="modal-overlay" onClick={() => setShowModal(false)}>
          <div className="modal-content" onClick={(e) => e.stopPropagation()}>
            <h2>New Invoice</h2>
            <form onSubmit={handleSubmit}>
              <div className="form-group">
                <label>Pacienti *</label>
                <select 
                  value={formData.patientId} 
                  onChange={(e) => setFormData({ ...formData, patientId: parseInt(e.target.value) })} 
                  required
                >
                  <option value={0}>Zgjidhni Pacientin</option>
                  {patients.map(patient => (
                    <option key={patient.patientId} value={patient.patientId}>
                      {patient.name}
                    </option>
                  ))}
                </select>
              </div>
              <div className="form-group">
                <label>Totali (ALL) *</label>
                <input 
                  type="number" 
                  step="0.01" 
                  min="0.01"
                  value={formData.total || ''} 
                  onChange={(e) => setFormData({ ...formData, total: parseFloat(e.target.value) || 0 })} 
                  required 
                />
              </div>
              <div className="form-group">
                <p style={{ color: '#7f8c8d', fontSize: '12px', margin: 0 }}>
                  Statusi do të vendoset automatikisht si "Pending"
                </p>
              </div>
              <div className="form-actions">
                <button type="button" onClick={() => setShowModal(false)} className="btn-secondary">Cancel</button>
                <button type="submit" className="btn-primary">Create</button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  )
}

