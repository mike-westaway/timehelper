import React, { useState } from 'react'
import './App.css';

import { useMsal, useMsalAuthentication, AuthenticatedTemplate } from "@azure/msal-react";
import { TextField, Spinner, Label, ProgressIndicator, SelectionMode, GroupHeader, DetailsList, Fabric, MessageBar, MessageBarType, PrimaryButton, Stack, DefaultButton, Separator, Dropdown, Slider, Panel, PanelType, } from '@fluentui/react'
import { Icon } from '@fluentui/react/lib/Icon';
import { initializeIcons } from '@fluentui/react/lib/Icons';
import { mergeStyles } from 'office-ui-fabric-react/lib/Styling';

import { TagPicker } from 'office-ui-fabric-react/lib/Pickers';

initializeIcons();


function TimeItem({ dismissPanel, _update, projects, outlook_categories, item, idx }) {

  const [error, setError] = useState(null)
  console.log(idx)
  console.log(item)
  console.log(projects)
  console.log(outlook_categories)

  const [input, handleInputChange] = useState({
    'hours': item ? item.hours : 1,
    'association_type': item && item.categories && item.categories.length > 0 ? 1 : 0,
    'day': item ? item.day : 0,
    'project': item ? item.project : "",
    'calId': item && item.calId,
    'subject': item ? item.subject : "",
    'categories': item ? item.categories : []
  })

  function _onChange(e, val) {
    console.log(`onChange ${val}`)
    handleInputChange({
      ...input,
      [e.target.name]: val
    })
  }

  function onProjectSelected(i) {
    _onChange({ target: { name: "project" } }, i.name)
    return i
  }

  return (
    <Stack tokens={{ childrenGap: 15 }} >

      <Dropdown label="Day" defaultSelectedKey={input.day} onChange={(e, i) => _onChange({ target: { name: "day" } }, i.key)} options={[{ key: 0, text: "Sunday" }, { key: 1, text: "Monday" }, { key: 2, text: "Tuesday" }, { key: 3, text: "Wednesday" }, { key: 4, text: "Thursday" }, { key: 5, text: "Friday" }, { key: 6, text: "Saturday" }]} />

      <Label >Project Search</Label>
      <TagPicker
        label="Project"
        inputProps={{ defaultVisibleValue: input.project }}
        removeButtonAriaLabel="Remove"
        onResolveSuggestions={(filterText, tagList) => {
          return filterText
            ? projects.map(item => ({ key: item, name: item })).filter(tag => tag.name.toLowerCase().indexOf(filterText.toLowerCase()) >= 0)
            : []
        }}
        getTextFromItem={(item) => item.name}
        pickerSuggestionsProps={{
          suggestionsHeaderText: 'Suggested projects',
          noResultsFoundText: 'No projects found',
        }}
        itemLimit={2}
        onItemSelected={onProjectSelected}
      />

      <Slider
        label="Hours"
        min={1}
        max={10}
        step={1}
        defaultValue={input.hours}
        showValue={true}
        onChange={(val) => _onChange({ target: { name: "hours" } }, val)}
        snapToStep
      />

      <Label>Outlook Assosiation</Label>
      <Stack tokens={{ childrenGap: 15, padding: 10 }} styles={{ root: { border: "1px solid" } }}>

        <Dropdown label="Always assosiate project when" defaultSelectedKey={input.association_type} onChange={(e, i) => _onChange({ target: { name: "association_type" } }, i.key)} options={[{ key: 0, text: "Subject includes" }, { key: 1, text: "Catorgorised as" }]} />
        {input.association_type === 0 ?
          <TextField label="Subject" iconProps={{ iconName: 'Calendar' }} placeholder={input.subject} />
          : [
            <Label key="cat" >Categories</Label>,
            <TagPicker
              key="Cat"
              removeButtonAriaLabel="Remove"
              inputProps={{ defaultVisibleValue: input.categories.length > 0 ? input.categories[0] : "" }}
              onResolveSuggestions={(filterText, tagList) => {
                return filterText
                  ? outlook_categories.map(item => ({ key: item, name: item })).filter(tag => tag.name.toLowerCase().indexOf(filterText.toLowerCase()) >= 0)
                  : []
              }}
              getTextFromItem={(item) => item.name}
              pickerSuggestionsProps={{
                suggestionsHeaderText: 'Suggested outlook categories',
                noResultsFoundText: 'No categories found',
              }}
              itemLimit={2}
              onItemSelected={(i) => {
                _onChange({ target: { name: "categories" } }, [i.name])
                return i
              }}
            />
          ]
        }
      </Stack>
      <Stack.Item>
        <Label>Outlook Id</Label>
        {input.calId}
      </Stack.Item>


      {error &&
        <MessageBar messageBarType={MessageBarType.error} isMultiline={false} truncated={true}>
          {error}
        </MessageBar>
      }
      <Stack horizontal tokens={{ childrenGap: 5 }}>
        <PrimaryButton text="Save" onClick={() => _update(idx, input)} allowDisabledFocus disabled={false} />
        <DefaultButton text="Cancel" onClick={dismissPanel} allowDisabledFocus disabled={false} />
        {idx >= 0 && <DefaultButton text="Delete" onClick={() => _update(idx, null)} allowDisabledFocus disabled={false} />}
      </Stack>

    </Stack>
  )
}

const iconClass = mergeStyles({
  fontSize: 20,
  height: 20,
  width: 20,
  marginRight: '5px',
});

const flexrow = mergeStyles({
  width: "100%",
  display: "flex",
  flexDirection: "row",
  justifyContent: "left"
})

function App() {

  const [panel, setPanel] = React.useState({ open: false })
  const [importcal, setImportcal] = React.useState(0)
  const [projects, setProjects] = React.useState([
    'Project ACE',
    'Project Lighthouse'])

  const [categories, setCategories] = React.useState([])

  const [entries, setEntries] = React.useState({
    hours: 0,
    groups: [
      { key: 'sun', name: 'Sunday', startIndex: 0, count: 0, level: 0, hours: 0 },
      { key: 'mon', name: 'Monday', startIndex: 0, count: 0, level: 0, hours: 0 },
      { key: 'tue', name: 'Tuesday', startIndex: 0, count: 0, level: 0, hours: 0 },
      { key: 'wed', name: 'Wednesday', startIndex: 0, count: 0, level: 0, hours: 0 },
      { key: 'thu', name: 'Thursday', startIndex: 0, count: 0, level: 0, hours: 0 },
      { key: 'fri', name: 'Friday', startIndex: 0, count: 0, level: 0, hours: 0 },
      { key: 'sat', name: 'Saturday', startIndex: 0, count: 0, level: 0, hours: 0 }
    ],
    items: []
  })

  useMsalAuthentication("redirect");
  const { instance, accounts } = useMsal();


  function openTimeItem(editid) {
    setPanel({ open: true, editid })
  }

  function dismissPanel() {
    setPanel({ open: false })
  }

  function _sumbit() {

  }

  function add_in_order(new_val, array) {

    // remove existing item with same calId
    const existing_idx = new_val.calId ? array.findIndex(i => new_val.calId === i.calId) : -1
    const items = existing_idx >= 0 ? [...array.slice(0, existing_idx), ...array.slice(existing_idx + 1)] : [...array]

    // add new in order
    for (let i = 0; i < items.length; i++) {
      if (new_val.day <= items[i].day) {
        return [...items.slice(0, i), new_val, ...items.slice(i)]
      }
    }
    return [...items, new_val]
  }

  function add(idx, item) {

    setEntries((prevState) => {
      if (idx >= 0) { // editing exiting item
        if (item) { //changing
          return recalcGroups([...prevState.items.slice(0, idx), item, ...prevState.items.slice(idx + 1)], prevState.groups)
        } else { // deleting
          return recalcGroups([...prevState.items.slice(0, idx), ...prevState.items.slice(idx + 1)], prevState.groups)
        }
      } else {
        return recalcGroups(add_in_order(item, [...prevState.items]), prevState.groups)
      }
    })
    dismissPanel()
  }

  function recalcGroups(items, currentGroups) {

    const hours = items.reduce((a, i) => a + (i.project ? i.hours : 0), 0)
    const groups = items.reduce((g, i) => {
      if (!i.project) {
        g[i.day].issue = true
      }
      g[i.day].count++; g[i.day].hours += i.hours
      for (let a = i.day + 1; a < g.length; a++) {
        g[a].startIndex++
      }
      return g
    }, currentGroups.map(g => { return { ...g, startIndex: 0, count: 0, hours: 0, issue: false } }))

    //console.log('groups')
    //console.log(groups)
    return { items, groups, hours }
  }

  function additems(newitems) {

    const ps = new Set(projects)
    const cs = new Set(categories)

    setEntries((prevState) => {

      let items = [...prevState.items]

      for (const n of newitems) {
        if (n.project) {
          ps.add(n.project)
        }
        if (n.categories) {
          for (let cat of n.categories)
            cs.add(cat)
        }
        items = add_in_order(n, items)
      }
      console.log('items')
      console.log(items)

      return recalcGroups(items, prevState.groups)
    })
    setProjects(Array.from(ps))
    setCategories(Array.from(cs))
  }

  function _callAPI() {
    setImportcal(1)
    if (accounts && accounts.length > 0) {
      instance.acquireTokenSilent({
        scopes: [process.env.REACT_APP_SERVER_SCOPE],
        account: accounts[0]
      }).then((response) => {
        if (response) {

          fetch(process.env.REACT_APP_SERVER_CALURL, {
            headers: {
              'Authorization': `Bearer ${response.accessToken}`,
              'Content-Type': 'application/json'
            },
            mode: 'cors'
          }).then(response => response.json())
            .then(data => {

              additems(data.events.map(i => {
                const d = new Date(i.startTime) //new Date(i.startTime.substr(6, 4), i.startTime.substr(3, 2) - 1, i.startTime.substr(0, 2), 1)
                console.log(`adding ${i.subject}  -- ${i.startTime} -- ${d.getDay()} -- ${i.durationInMinutes}`)
                return {
                  calId: i.iCalUId,
                  project: i.project,
                  hours: Number.parseFloat((i.durationInMinutes / 60).toFixed(1)),
                  day: d.getDay(),
                  subject: i.subject,
                  categories: i.categories
                }
              })//.filter(f => f.day >= 0 && f.day <= 4)
              )
              setImportcal(0)
            })
        }
      })
    }
  }

  function submitts() {
    //setError(null)
    //_fetchit('/api/store/inventory', 'POST', {}, result._id ? { _id: result._id, ...input } : input).then(succ => {
    //  console.log(`created success : ${JSON.stringify(succ)}`)
    //navTo("/MyBusiness")
    //dismissPanel()
    //}, err => {
    //  console.error(`created failed : ${err}`)
    //  setError(`created failed : ${err}`)
    //})
  }

  return (
    <Fabric>
      <AuthenticatedTemplate>
        <main id="mainContent" data-grid="container" >

          <nav className="header">

            <div className="logo" style={{ padding: "6px 0" }}>
              <Icon iconName="TimeEntry" style={{ fontSize: 23, margin: '0 15px', color: 'deepskyblue' }} />
            </div>
            <div className="logo" style={{ padding: "8px 0" }}>
              <div style={{ fontSize: 15 }}>Time Assistant for <b>{accounts && accounts.length > 0 ? accounts[0].name : "unknown"}</b></div>
            </div>
            <input className="menu-btn" type="checkbox" id="menu-btn" />
            <label className="menu-icon" htmlFor="menu-btn"><span className="navicon"></span></label>
            <ul className="menu">
              <li style={{ cursor: "pointer" }}><a  >Time Entry</a></li>
              <li style={{ cursor: "pointer" }}><a >My Projects</a></li>
              <li style={{ cursor: "pointer" }}><a >My Analytics</a></li>
              <li style={{ cursor: "pointer" }}><a onClick={() => instance.logout()}>Logout</a></li>
            </ul>
          </nav>

          <div style={{ "height": "43px", "width": "100%" }} />


          <Stack className="wrapper" tokens={{ childrenGap: 10, padding: 10, maxWidth: "900px" }}>

            <Panel
              headerText="New Time entry"
              isOpen={panel.open}
              onDismiss={dismissPanel}
              type={PanelType.small}
              // You MUST provide this prop! Otherwise screen readers will just say "button" with no label.
              closeButtonAriaLabel="Close"
            >
              {panel.open &&
                <TimeItem dismissPanel={dismissPanel} _update={add} {...panel} projects={projects} outlook_categories={categories} />
              }
            </Panel>

            <Stack horizontal tokens={{ childrenGap: 40, padding: 10 }}>
              <div style={{ height: "65px", width: "65px" }} className="ms-BrandIcon--icon96 ms-BrandIcon--outlook"></div>

              <DefaultButton style={{ margin: '15px 15px' }} iconProps={{ iconName: 'CalendarWeek' }} text="Import from events" onClick={_callAPI} disabled={(importcal !== 0)} />
              {importcal === 1 &&
                <Spinner style={{ marginLeft: "0px" }} label="loading..." />
              }
            </Stack>

            <Separator></Separator>
            <DefaultButton iconProps={{ iconName: 'Add' }} text="Create Time Entry" styles={{ root: { width: 180 } }} onClick={() => openTimeItem()} />

            <DetailsList

              items={entries.items}
              groups={entries.groups}
              columns={[
                { key: 'day', name: `Week (${entries.hours} hrs)`, minWidth: 100, maxWidth: 200, isResizable: false },
                {
                  key: 'project', name: 'Project', fieldName: 'project', minWidth: 100, maxWidth: 200, onRender: (i) => {
                    if (i.project) {
                      return <Label>{i.project}</Label>
                    } else {
                      return <div key={i} className={flexrow}>
                        <Icon iconName="Warning" className={iconClass} style={{ color: "red" }} />
                        <div>Add Project</div>
                      </div>
                    }
                  }
                },
                { key: 'hours', name: 'Hours', fieldName: 'hours', minWidth: 100, maxWidth: 200, onRender: (i) => <Label>{i.hours} hrs</Label> },
                {
                  key: 'cat', name: 'Outlook entry', minWidth: 100, maxWidth: 200, onRender: (i) =>
                    <div>
                      {i.subject}
                      {i.categories && i.categories.map((c, i) =>
                        <div key={i} className={flexrow}>
                          <Icon iconName="Tag" className={iconClass} style={{ color: "red" }} />
                          <div>{c}</div>
                        </div>)
                      }
                    </div>
                }
              ]}
              ariaLabelForSelectAllCheckbox="Toggle selection for all items"
              ariaLabelForSelectionColumn="Toggle selection"
              checkButtonAriaLabel="Row checkbox"
              selectionMode={SelectionMode.none}
              groupProps={{
                showEmptyGroups: true,
                onRenderHeader: (item) => <GroupHeader onRenderGroupHeaderCheckbox={false} {...item} onRenderTitle={(i) =>
                  <div className='ms-GroupHeader-title' role="gridcell">
                    {i.group.issue &&
                      <Icon iconName="Warning" className={iconClass} style={{ color: "red" }} />
                    }
                    <span>{i.group.name}  ({i.group.hours} hrs
                    {i.group.hasMoreData && '+'})
                </span>
                  </div>} />
              }}
              onActiveItemChanged={(item, idx) => setPanel({ open: true, item, idx })}
              compact={false}
            />

            <ProgressIndicator label={`${Number.parseFloat((entries.hours / 40) * 100).toFixed(0)}% Complete (${entries.hours} hrs)`} percentComplete={entries.hours / 40} barHeight={entries.hours} />

            <Stack.Item align="end">
              <DefaultButton text="Submit" onClick={_sumbit} allowDisabledFocus disabled={entries.hours < 40} />
            </Stack.Item>
          </Stack>

        </main>
      </AuthenticatedTemplate>
    </Fabric>
  )
}

export default App;
