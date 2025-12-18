import { Calendar } from '@fullcalendar/core'
import timeGridPlugin from '@fullcalendar/timegrid'
import dayGridPlugin from '@fullcalendar/daygrid'
import iCalendarPlugin from '@fullcalendar/icalendar'

import '@fullcalendar/core/index.css'
import '@fullcalendar/daygrid/index.css'
import '@fullcalendar/timegrid/index.css'


function init() {
  const el = document.getElementById('calendar')
  if (!el) return

  const feedUrl = el.dataset.feedUrl || '/schedules/feed.ics'

  const calendar = new Calendar(el, {
    plugins: [timeGridPlugin, dayGridPlugin, iCalendarPlugin],
    initialView: 'timeGridWeek',
    height: 800,
    expandRows: true,
    headerToolbar: {
      left: 'prev,next today',
      center: 'title',
      right: 'dayGridMonth,timeGridWeek,timeGridDay'
    },
    slotMinTime: '09:00:00',
    slotMaxTime: '23:00:00',
    nowIndicator: true,

    eventSources: [
      { url: feedUrl, format: 'ics' }
    ]
  })

  calendar.render()
}

document.addEventListener('DOMContentLoaded', init)
